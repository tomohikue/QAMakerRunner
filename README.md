# QAMakerRunner
QnA Makerのテスト＆結果ログ出力ツール

- QnA MakerからExportしたKBファイルをそのままインプットファイルにしてテストが実行できます
- 結果ファイルはScore順にトップ３の回答とScoreがCSV形式で出力されます。

## 使い方

①Visual Studioでソリューションファイル「QAMakerRunner.sln」を開く

②App.configを修正する

~~~xml
  <appSettings>
    <!--QnA Makerの設定-->
    <add key="QnAMakerUrl" value="https://tomohikuxxxx.azurewebsites.net/qnamaker/knowledgebases/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxx/generateAnswer" />
    <add key="QnAMakerApiKey" value="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxx" />
    <!--QnA MakerのKBファイルのパス-->
    <add key="InputFilePath" value="D:\temp\inputKb.tsv" />
    <!--結果ファイルのパス-->
    <add key="OutFilePath" value="D:\temp\QnAMakerResult.csv" />
    <!--以下のKeyは値なしでOK-->
    <add key="ClientSettingsProvider.ServiceUri" value="" />
  </appSettings>
~~~
③ビルドして実行
