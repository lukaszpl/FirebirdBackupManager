using CommonClassLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FirebirdBackupManager.Logic
{
    internal class ResultCheckerThread
    {
        string currentMD5 = "";
        public void CheckChangesInResultFileLoop()
        {
            string pathToResultsFile = App.jsonDirectory + "//results.json";
            currentMD5 = CalculateMD5(pathToResultsFile);
            while (true)
            { 
                string newMD5 = CalculateMD5(pathToResultsFile);
                if (!currentMD5.Equals(newMD5))
                {
                    currentMD5 = newMD5;
                    Application.Current.Dispatcher.Invoke(() => ReloadResults());
                }
                Thread.Sleep(5000);
            }
        }

        private void ReloadResults()
        {
            if (File.Exists(App.jsonDirectory + "\\results.json"))
            {
                GlobalListsData.taskResults = JsonConvert.DeserializeObject<List<TaskResult>>(File.ReadAllText(App.jsonDirectory + "\\results.json")); ;
                foreach (var taskResult in GlobalListsData.taskResults)
                {
                    foreach (var taskItem in GlobalListsData.taskItemsList)
                    {
                        if (taskResult.taskId == taskItem.Id)
                        {
                            taskItem.lastResult = taskResult.lastResult;
                            taskItem.lastActive = taskResult.lastActive.ToString();
                        }
                    }
                }
            }
        }

        private string CalculateMD5(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }
                }
            }
            else
            {
                return "";
            }
        }
    }
}
