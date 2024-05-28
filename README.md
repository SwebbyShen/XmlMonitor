README
This README would normally document whatever steps are necessary to get your application up and running.

機能説明
XMLファイル（パス指定可能）が作成された度にbatファイル（パス指定可能）を起動することができる。

パラメータの説明
logFilePath //ログファイルのパス

monitorPath //作成されたXMLファイルのパス（監視パス）

batFilePath //batファイルのパス

toMail //宛先のメールアドレス

mailTitle //送信メールのタイトル

mailBody //送信メールの内容

環境設定
「コントロール パネル」⇒「システムとセキュリティ」⇒「システム」⇒「システムの詳細設定」⇒「環境変数」⇒ 「XXXXXXのユーザー環境変数」の「変数」列の「Path」行をダブルクリック ⇒ C:\Windows\Microsoft.NET\Framework\v4.0.30319を新規追加 ⇒「OK」

「コントロール パネル」⇒「システムとセキュリティ」⇒「システム」⇒「システムの詳細設定」⇒「環境変数」⇒ 「システム環境変数」⇒「新規」をクリックすることで下記の変数の値を設定する必要がある。

logFilePath

monitorPath

batFilePath

toMail

mailTitle

mailBody

Windows Serviceへのインストール方法
コマンドプロンプトを管理者として実行（重要）⇒ InstallUtil C:\Users\XXXXXX\Desktop\xmlmonitor\xmlmonitor\bin\Debug\xmlmonitor.exeを実行（XXXXXXは実際のパスに変更してください）

Windowsの「サービス」アプリには、AsImport_purchaseinvoiceというサービスがあればインストール成功。

右クリックするとサービスの起動や停止操作は可能です。

Author
Hang Shen