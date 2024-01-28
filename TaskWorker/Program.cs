using CommonClassLibrary;
using CommonClassLibrary.Models;
using CommonClassLibrary.Models.Storage;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;

namespace TaskWorker
{
    internal class Program
    {    
        static string pathToGbak = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\firebird\\3.0.10\\gbak.exe";
        static int taskId = -1;


        static void Main(string[] args)
        {
            if (CheckAndLoadInputData(args) && CheckFilesExists()) //load input data (args) and check conf files exists
            {
                JsonLoader.LoadDataFromJson(); //load data (FbServers, Storage, Tasks)
                JsonLoader.LoadResults(); //load results if exists
                JsonLoader.LoadNotificationSettings(); //load email settings if exists
                StartTask(taskId);
            }
            else
            {
                Environment.Exit(100);
            }
        }


        private static bool CheckAndLoadInputData(string[] args)
        {
            try
            {
                taskId = Convert.ToInt32(args[0]);             
            }
            catch (Exception e)
            {
                Console.WriteLine("Id is not a number! Use: TaskWorker.exe id");
                return false;
            }
            return true;
        }

        private static bool CheckFilesExists()
        {
            if (Directory.Exists(JsonLoader.jsonDirectory)){
                if (!File.Exists(JsonLoader.backupStorageFilePath)) {
                    Console.WriteLine("missing file: backupStorage.json");
                    return false;
                }
                if (!File.Exists(JsonLoader.FbServersFilePath))
                {
                    Console.WriteLine("missing file: FbServers.json");
                    return false;
                }
                if (!File.Exists(JsonLoader.tasksFilePath))
                {
                    Console.WriteLine("missing file: tasks.json");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("missing directory: " + JsonLoader.jsonDirectory);
                return false;
            }
            return true;
        }


        static string output = "Firebird Backup Manager log file:\n";
        private static void StartTask(int taskId)
        {
            foreach(TaskItem task in JsonLoader.tasks)
            {
                if(task.Id == taskId)
                {
                    string storagePath = GetStoragePathById(task.storageId);
                    //create folder in storage path
                    string pathToTaskSubfolder = CreateFolderForTask(storagePath, task).Replace("\\\\", "\\");
                    //
                    string parameters = $"-b -v -user {GetServerUserById(task.fBServerId)} -password {GetServerPasswordById(task.fBServerId)} {GetServerIPById(task.fBServerId)}:{task.databasePath} \"{pathToTaskSubfolder + "\\" + DateTime.Now.ToString("dd-MM-yyy-HH-mm") + ".gbk"}\"";
                    //start, save status when is running
                    SaveTaskResult(taskId, 0, "running...");
                    //
                    var procGbak = new Process();
                    procGbak.StartInfo.FileName = pathToGbak;
                    procGbak.StartInfo.RedirectStandardOutput = true;
                    procGbak.StartInfo.RedirectStandardError = true;
                    procGbak.StartInfo.UseShellExecute = false;
                    procGbak.StartInfo.Arguments = parameters;
                    procGbak.Start();
                    procGbak.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                    procGbak.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
                    procGbak.BeginOutputReadLine();
                    procGbak.BeginErrorReadLine();
                    procGbak.WaitForExit();                     
                    Console.WriteLine("Result: " + procGbak.ExitCode);                
                    //save result
                    SaveTaskResult(taskId, procGbak.ExitCode, "complete");
                    //delete old backups, if new backup is created
                    if(procGbak.ExitCode == 0)
                    {
                        DeleteOldBackups(pathToTaskSubfolder, task.retentionTime);
                    }
                    else
                    {
                        //if error, save log
                        SaveLog(output, pathToTaskSubfolder);
                        //if backup is not created, send info to email
                        SendEmail("Task Error: " + task.taskName + " error code: " + procGbak.ExitCode + " Time: " + DateTime.Now);
                    }
                    Environment.Exit(procGbak.ExitCode);
                }
            }
        }
        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            output += outLine.Data + "\n";
        }
        private static void SaveTaskResult(int taskId, int lastResult, string status)
        {
            string lastResultString = "";
            if (!status.Equals("running..."))
            {
                if (lastResult == 0)
                {
                    lastResultString = "Success";
                }
                else
                {
                    lastResultString = "error code: " + lastResult;
                }
            }
            else
            {
                lastResultString = "running...";
            }
            //
            if (JsonLoader.taskResults != null)
            {
                bool taskExists = false;
                foreach(var taskResult in JsonLoader.taskResults)
                {
                    if (taskResult.taskId == taskId)
                    {
                        taskExists = true; //task exists
                        taskResult.lastResult = lastResultString;
                        taskResult.lastActive = DateTime.Now;
                        taskResult.status = status;
                    }
                }
                //if it is new task, create it
                if(!taskExists)
                    JsonLoader.taskResults.Add(new TaskResult(taskId, DateTime.Now, lastResultString, status));
            }
            else
            {
                JsonLoader.taskResults = new List<TaskResult>();
                JsonLoader.taskResults.Add(new TaskResult(taskId, DateTime.Now, lastResultString, status));
            }
            JsonLoader.SaveResult(JsonLoader.jsonDirectory + "\\results.json");
        }

        private static void SaveLog(string log, string pathToTaskSubfolder)
        {
            if (!Directory.Exists(pathToTaskSubfolder + "\\logs"))
            {
                Directory.CreateDirectory(pathToTaskSubfolder + "\\logs");
            }
            File.WriteAllText(pathToTaskSubfolder + "\\logs\\" + DateTime.Now.ToString("dd-MM-yyy-HH-mm") + ".log", log);
        }
        private static string GetServerIPById(int id)
        {
            foreach(var item in JsonLoader.Fbservers)
            {
                if(item.id == id)
                {
                    return item.serverIP;
                }
            }
            return null;
        }
        private static string GetServerUserById(int id)
        {
            foreach (var item in JsonLoader.Fbservers)
            {
                if (item.id == id)
                {
                    return item.user;
                }
            }
            return null;
        }
        private static string GetServerPasswordById(int id)
        {
            foreach (var item in JsonLoader.Fbservers)
            {
                if (item.id == id)
                {
                    return item.password;
                }
            }
            return null;
        }
        private static string GetStoragePathById(int id)
        {
            foreach (var item in JsonLoader.backupStorageInfos)
            {
                if (item.id == id)
                {
                    return item.storagePath;
                }
            }
            return null;
        }

        private static string CreateFolderForTask(string storagePath, TaskItem taskItem)
        {
            if (Directory.Exists(storagePath))
            {
                string pathToTaskSubfolder = storagePath + "\\" + taskItem.taskName + " " + taskItem.Id;
                if (!Directory.Exists(pathToTaskSubfolder))
                {
                    Directory.CreateDirectory(pathToTaskSubfolder);
                }
                return pathToTaskSubfolder;
            }
            else
            {
                Console.WriteLine("Storage path is invalid!");
                Environment.Exit(102);
            }
            return "";
        }

        private static void DeleteOldBackups(string pathToTaskSubfolder, int retentionTimeDays)
        {
            string[] Files = Directory.GetFiles(pathToTaskSubfolder);
            foreach (string file in Files)
            {
                if(File.GetCreationTime(file).Date < DateTime.Now.AddDays(retentionTimeDays * (-1))){
                    File.Delete(file);
                }
            }
        }

        private static void SendEmail(string InfoString)
        {
            if(JsonLoader.mailConfig != null)
            {
                MailSender.SendEmail(JsonLoader.mailConfig.smtpServer, Convert.ToInt32(JsonLoader.mailConfig.smtpPort), JsonLoader.mailConfig.smtpUserName, JsonLoader.mailConfig.smtpPassword, JsonLoader.mailConfig.SSL, JsonLoader.mailConfig.receiver, "Task Error - Firebird Backup Manager", InfoString);
            }
        }
    }
}
