using CommonClassLibrary.Models;
using CommonClassLibrary.Models.Storage;
using FirebirdBackupManager.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FirebirdBackupManager.Views
{
    /// <summary>
    /// Logika interakcji dla klasy AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        bool isEditMode = false;
        ListViewTaskItem taskItemToEdit;
        public AddTaskWindow()
        {
            isEditMode = false;
            InitializeComponent();
            FbServers_comboBox.ItemsSource = GlobalListsData.fbServersList;
            storage_comboBox.ItemsSource = GlobalListsData.backupStorageInfoList;
        }

        //edit mode
        public AddTaskWindow(ListViewTaskItem taskItem)
        {
            isEditMode = true;
            this.taskItemToEdit = taskItem;
            InitializeComponent();
            FbServers_comboBox.ItemsSource = GlobalListsData.fbServersList;
            storage_comboBox.ItemsSource = GlobalListsData.backupStorageInfoList;
            LoadCurrentSettings(taskItem);
            taskName_textBox.IsEnabled = false;
            storage_comboBox.IsEnabled = false;
            FbServers_comboBox.IsEnabled = false;
        }

        private void LoadCurrentSettings(ListViewTaskItem taskItem)
        {
            taskName_textBox.Text = taskItem.taskName;
            databasePath_textBox.Text = taskItem.databasePath;
            comment_textBox.Text = taskItem.comment;
            timeHour_textBox.Text = taskItem.timeHour;
            retentionTime_textBox.Text = taskItem.retentionTime.ToString();
            foreach(var FbServer in GlobalListsData.fbServersList)
            {
                if(FbServer.id == taskItem.fBServerId)
                {
                    FbServers_comboBox.SelectedItem = FbServer; break;
                }
            }
            foreach (var Storage in GlobalListsData.backupStorageInfoList)
            {
                if (Storage.id == taskItem.storageId)
                {
                    storage_comboBox.SelectedItem = Storage; break;
                }
            }
        }

        private void Save_button_Click(object sender, RoutedEventArgs e)
        {
            FBServerInfo fBServerInfo = (FBServerInfo)FbServers_comboBox.SelectedItem;
            BackupStorageInfo backupStorageInfo = (BackupStorageInfo)storage_comboBox.SelectedItem;
            ScheduleType scheduleType = ScheduleType.Everyday;
            if (scheduleType_comboBox.SelectedIndex == 0)
                scheduleType = ScheduleType.Everyday;
            string databasePath = databasePath_textBox.Text;
            string timeHour = timeHour_textBox.Text;
            int retentionTime = Convert.ToInt32(retentionTime_textBox.Text);
            if (fBServerInfo != null && backupStorageInfo != null && databasePath != "" && timeHour != "")
            {
                //test connection to Db
                if (FirebirdConnection.testConnection(fBServerInfo.serverIP, fBServerInfo.serverPort.ToString(), fBServerInfo.user, fBServerInfo.password, databasePath))
                {
                    if (!isEditMode)
                    {
                        ListViewTaskItem listViewTaskItem = new ListViewTaskItem(getNewTaskId(), taskName_textBox.Text, DateTime.Now, "", "", comment_textBox.Text, scheduleType, timeHour, retentionTime, fBServerInfo.id, backupStorageInfo.id, databasePath, fBServerInfo.serverIP);
                        //schedule task
                        if (addTaskToTaskScheduler(listViewTaskItem))
                        {
                            GlobalListsData.taskItemsList.Add(listViewTaskItem);
                        }
                        else
                        {
                            MessageBox.Show(Languages.Lang.scheduleError, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        //delete task from taskscheduler
                        AppTaskScheduler scheduler = new AppTaskScheduler();
                        scheduler.DeleteTask("ID" + taskItemToEdit.Id);
                        
                        taskItemToEdit.taskName = taskName_textBox.Text;
                        taskItemToEdit.serverIP = fBServerInfo.serverIP;
                        taskItemToEdit.fBServerId = fBServerInfo.id;
                        taskItemToEdit.storageId = backupStorageInfo.id;
                        taskItemToEdit.comment = comment_textBox.Text;
                        taskItemToEdit.timeHour = timeHour;
                        taskItemToEdit.scheduledAt = ""; //fake change for OnPropertyChanged
                        taskItemToEdit.retentionTime = retentionTime;
                        taskItemToEdit.databasePath = databasePath;
                        //add new task, and schedule it
                        if (!addTaskToTaskScheduler(taskItemToEdit))
                        {
                            MessageBox.Show(Languages.Lang.scheduleError, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    //save to file
                    string json = JsonConvert.SerializeObject(GlobalListsData.taskItemsList);
                    File.WriteAllText(App.jsonDirectory + "\\tasks.json", json);
                    //
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show(Languages.Lang.missingFields, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private int getNewTaskId()
        {
            return GlobalListsData.taskItemsList.Count();
        }

        private bool addTaskToTaskScheduler(ListViewTaskItem task)
        {
            AppTaskScheduler scheduler = new AppTaskScheduler();
            string pathToApp = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\TaskWorker.exe";
            return scheduler.CreateTask("ID" + task.Id, task.timeHour, pathToApp, task.Id.ToString());
        }

    }
}
