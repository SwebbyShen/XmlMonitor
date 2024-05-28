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

namespace xmlmonitor
{
    public partial class Service1 : ServiceBase
    {
        private FileSystemWatcher watcher;        

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            DateTime currentDateTime = DateTime.Now;
            string logFilePath = @"C:\Users\1004739\Desktop\battest\xmlfolder\MyServiceLog.txt";
            string monitorpath = @"C:\Users\1004739\Desktop\battest\xmlfolder";
            watcher = new FileSystemWatcher(monitorpath);
                watcher.Filter = "*.xml";
                watcher.Created += (sender, e) =>
                {
                    string xmlFilePath = e.FullPath;
                    File.AppendAllText(logFilePath, $"Detected XML file: {xmlFilePath}");
                    string batFilePath = @"C:\Users\1004739\Desktop\battest\bat.bat";
                    ExecuteBatFile(batFilePath);
                };
                watcher.EnableRaisingEvents = true;
                File.AppendAllText(logFilePath, $"Service started. {currentDateTime}");
                Console.ReadLine();            
        }

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        private void ExecuteBatFile(string batFilePath)
        {
            string logFilePath = @"C:\Users\1004739\Desktop\battest\xmlfolder\MyServiceLog.txt";
            try
            {
                File.AppendAllText(logFilePath, "Executing Batch File.");
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
                EventLog.WriteEntry("xmlservice", $"Execution result: {output}", EventLogEntryType.Information);
                File.AppendAllText(logFilePath, $"Execution result: {output}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error executing .bat file: {ex.Message}");
            }
        }
    }
}
