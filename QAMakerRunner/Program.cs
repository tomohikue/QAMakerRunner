using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Configuration;

namespace QAMakerRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string QnAMakerUrl = ConfigurationManager.AppSettings["QnAMakerUrl"];
                string QnAMakerApiKey = ConfigurationManager.AppSettings["QnAMakerApiKey"];
                string InputFilePath = ConfigurationManager.AppSettings["InputFilePath"];
                string OutFilePath = ConfigurationManager.AppSettings["OutFilePath"];

                List<string> outdata = new List<string>();
                outdata.Add("InputWord\tCorrectWord\tPredictionWord1\tScore1\tPredictionWord2\tScore2\tPredictionWord3\tScore3\tPredictionWord4\tScore4\tPredictionWord5\tScore5");

                List<InputData> InputData = new List<InputData>();

                TextFieldParser textFieldParser = new TextFieldParser(InputFilePath);
                using (textFieldParser)
                {
                    textFieldParser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                    textFieldParser.SetDelimiters("\t");

                    Dictionary<string, int> headerDictionary = new Dictionary<string, int>();
                    Dictionary<string, int> Word2IndexDictionary = new Dictionary<string, int>();
                    int index = 0;
                    foreach (string header in textFieldParser.ReadFields())
                    {
                        headerDictionary.Add(header, index);
                        index += 1;
                    }

                    while (!textFieldParser.EndOfData)
                    {
                        string[] csvRow = textFieldParser.ReadFields();
                        InputData.Add(new InputData
                        {
                            Question = csvRow[0],
                            Answer = csvRow[1],
                            Source = csvRow[2],
                            Metadata = csvRow[3]
                        });
                    }
                }

                //文字コードを指定する
                Encoding enc = Encoding.GetEncoding("utf-8");


                ResponseModel res = new ResponseModel();

                int counter = 1;
                foreach (var i in InputData)
                {
                    // 特定の件数ごとにWaitをいれる場合 ※Peview時は1分に10件までであった
                    //if (counter % 10 == 0)
                    //{
                    //    Console.WriteLine("Wait 60000 sec");
                    //    System.Threading.Thread.Sleep(60000);
                    //}

                    //POST送信する文字列を作成
                    string postData =
                        "{\"question\":\"" + i.Question + "\",\"top\": 5}";
                    //HttpUtility.UrlEncode(i.Question, enc) + "\"}";

                    WebClient wc = new WebClient();
                    //文字コードを指定する
                    wc.Encoding = enc;
                    //ヘッダにContent-Typeを加える
                    wc.Headers.Add("Authorization", "EndpointKey " + QnAMakerApiKey);
                    wc.Headers.Add("Content-Type", "application/json");
                    //データを送信し、また受信する
                    string resText = wc.UploadString(QnAMakerUrl, postData);
                    wc.Dispose();


                    MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(resText));
                    DataContractJsonSerializer jsonSerialier = new DataContractJsonSerializer(typeof(ResponseModel));
                    var recommendations = jsonSerialier.ReadObject(memoryStream);
                    res = (ResponseModel)recommendations;

                    string row = i.Question + "\t" + i.Answer;
                    foreach (var j in res.answers)
                    {
                        row = row + "\t" + j.answer.Replace("\n","\\n") + "\t" + j.score;
                    };

                    outdata.Add(row);

                    //受信したデータを表示する
                    Console.WriteLine(counter + ": " + row);
                    counter++;
                }


                //テキストファイルを出力
                using (StreamWriter sw = new StreamWriter(OutFilePath, false, Encoding.UTF8))
                {
                    foreach(var i in outdata)
                    {
                        sw.WriteLine(i);
                    }

                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
        }

    }

    public class InputData
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Source { get; set; }
        public string Metadata { get; set; }

    }


    public class ResponseModel
    {
        public Answer[] answers { get; set; }
    }

    public class Answer
    {
        public string[] questions { get; set; }
        public string answer { get; set; }
        public float score { get; set; }
        public int id { get; set; }
        public string source { get; set; }
        public metadata[] metadata { get; set; }
    }
    public class metadata
    {
        public string name { get; set; }
        public string value { get; set; }
    }

}
