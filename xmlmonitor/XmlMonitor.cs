using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;
using System.Net.Mail;
using System.Net;

namespace xmlmonitor
{
    public partial class XmlMonitor : ServiceBase
    {
        List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        //private FileSystemWatcher watcher;
        string logVariableName = "logFilePath";
        string monitorVariableName = "monitorPath";
        string batVariableName = "batFilePath";
        string mailVariableName = "toMail";
        string mailTitleVariableName = "mailTitle";
        string mailBodyVariableName = "mailBody";
        string frommail = "frommail";
        string fromPwd = "mailPwd";

        public XmlMonitor()
        {
            ServiceName = "AsImport_purchaseinvoice";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            DateTime currentDateTime = DateTime.Now;
            string logFilePath = Environment.GetEnvironmentVariable(logVariableName);
            //ここで複数のパスを設定できる
            string[] monitorPaths = { Environment.GetEnvironmentVariable(monitorVariableName) };
            string batFilePath = Environment.GetEnvironmentVariable(batVariableName);
            
            //ログファイルがなければ生成
            if (!File.Exists(logFilePath))
            {
                using (File.Create(logFilePath)) { };
            }

            foreach (var path in monitorPaths)
            {
                var watcher = new FileSystemWatcher(path)
                {
                    Filter = "*.xml",
                    EnableRaisingEvents = true
                };
                watcher.Created += (sender, e) =>
                {
                    string xmlFilePath = e.FullPath;
                    File.AppendAllText(logFilePath, $" Detected XML file: {xmlFilePath}");                    
                    //バッチファイルの実行
                    ExecuteBatFile(batFilePath);
                };
                File.AppendAllText(logFilePath, $" Service started. {currentDateTime}");
                watchers.Add(watcher);
                Console.ReadLine();
                
            }
        }

        protected override void OnStop()
        {
            string logFilePath = Environment.GetEnvironmentVariable(logVariableName);

            string fromEmail = Environment.GetEnvironmentVariable(frommail);
            string fromPassword = Environment.GetEnvironmentVariable(fromPwd);

            string toEmail = Environment.GetEnvironmentVariable(mailVariableName);
            //string toEmail = "TRK101050-INFORMATION-GROUPWARE@jp.digi-group.com";

            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            MailMessage message = new MailMessage(fromEmail, toEmail)
            {
                Subject = Environment.GetEnvironmentVariable(mailTitleVariableName),
                //Subject = "Serviceが停止しました",
                Body = Environment.GetEnvironmentVariable(mailBodyVariableName),
                //Body = "AsImport_purchaseinvoiceが停止しました。",
            };
            try
            {
                client.Send(message);
                File.AppendAllText(logFilePath, $" Mail Sent. ");
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFilePath, $" Mail Sending Failed. {ex.Message}");
            }
            foreach (var watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }

        private void ExecuteBatFile(string batFilePath)
        {
            string logFilePath = Environment.GetEnvironmentVariable(logVariableName);
            try
            {
                File.AppendAllText(logFilePath, " Executing Batch File.");
                Process process = new Process();
                process.StartInfo.FileName = batFilePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.WriteLine();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                EventLog.WriteEntry("xmlservice", $" Execution result: {output}", EventLogEntryType.Information);
                File.AppendAllText(logFilePath, $" Execution result: {output}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($" Error executing .bat file: {ex.Message}");
            }
        }
    }
}
