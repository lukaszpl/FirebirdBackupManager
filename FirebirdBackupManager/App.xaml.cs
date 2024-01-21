using CommonClassLibrary.Models;
using CommonClassLibrary.Models.Storage;
using FirebirdBackupManager.Logic;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace FirebirdBackupManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string jsonDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FirebirdBackupManager";
        public App()
        {
            if(!Directory.Exists(jsonDirectory))
                Directory.CreateDirectory(jsonDirectory);
            GlobalListsData.fbServersList = new ObservableCollection<CommonClassLibrary.Models.FBServerInfo>();
            GlobalListsData.taskItemsList = new ObservableCollection<ListViewTaskItem>();
            GlobalListsData.backupStorageInfoList = new ObservableCollection<CommonClassLibrary.Models.Storage.BackupStorageInfo>();
            LoadSettings();
            startResultChecker();
            //to do, verify taskcheduler<->json
        }

        private void startResultChecker()
        {
            ResultCheckerThread resultCheckerThread = new ResultCheckerThread();
            Thread myThread = new Thread(resultCheckerThread.CheckChangesInResultFileLoop);
            myThread.Start();
        }

        private void LoadSettings()
        {
            try
            {
                if(File.Exists(jsonDirectory + "\\FbServers.json"))
                    GlobalListsData.fbServersList = JsonConvert.DeserializeObject<ObservableCollection<FBServerInfo>>(File.ReadAllText(jsonDirectory + "\\FbServers.json"));
                if (File.Exists(jsonDirectory + "\\backupStorage.json"))
                    GlobalListsData.backupStorageInfoList = JsonConvert.DeserializeObject<ObservableCollection<BackupStorageInfo>>(File.ReadAllText(jsonDirectory + "\\backupStorage.json"));
                if (File.Exists(jsonDirectory + "\\tasks.json"))
                    GlobalListsData.taskItemsList = JsonConvert.DeserializeObject<ObservableCollection<ListViewTaskItem>>(File.ReadAllText(jsonDirectory + "\\tasks.json"));
                //load results
                if (File.Exists(jsonDirectory + "\\results.json"))
                {
                    GlobalListsData.taskResults = JsonConvert.DeserializeObject<List<TaskResult>>(File.ReadAllText(jsonDirectory + "\\results.json")); ;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
