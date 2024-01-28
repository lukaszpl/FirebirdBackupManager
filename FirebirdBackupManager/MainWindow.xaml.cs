using CommonClassLibrary.Models;
using System.Globalization;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonClassLibrary.Models.Storage;
using static CommonClassLibrary.Models.Storage.BackupStorageInfo;
using System.Collections.ObjectModel;
using FirebirdBackupManager.Views;
using FirebirdBackupManager.Views.StorageWindows;
using FirebirdBackupManager.Logic;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32.TaskScheduler;
using System.DirectoryServices.ActiveDirectory;
namespace FirebirdBackupManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            tasks_listView.ItemsSource = GlobalListsData.taskItemsList;
        }


        private void AddTask_button_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow();
            addTaskWindow.ShowDialog();
        }

        private void MenuItem_ServerManager_Click(object sender, RoutedEventArgs e)
        {
            ServerListWindow serverListWindow = new ServerListWindow();
            serverListWindow.ShowDialog();
        }
        private void MenuItem_StorageSettings_Click(object sender, RoutedEventArgs e)
        {
            StorageListWindow storageListWindow = new StorageListWindow();
            storageListWindow.ShowDialog();
        }

        private void delete_Button_Click(object sender, RoutedEventArgs e)
        {
            ListViewTaskItem selectedTaskItem = (ListViewTaskItem)tasks_listView.SelectedItem;
            if (selectedTaskItem != null)
            {
                GlobalListsData.taskItemsList.Remove(selectedTaskItem);
                //delete task from taskscheduler
                AppTaskScheduler scheduler = new AppTaskScheduler();
                scheduler.DeleteTask("ID" + selectedTaskItem.Id);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string json = JsonConvert.SerializeObject(GlobalListsData.taskItemsList);
            File.WriteAllText(App.jsonDirectory + "\\tasks.json", json);
            Environment.Exit(0);
        }

        private void MenuItem_NotificationSettings_Click(object sender, RoutedEventArgs e)
        {
            NotificationsSettingsWindow notificationSettingsWindow = new NotificationsSettingsWindow();
            notificationSettingsWindow.ShowDialog();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void EditTask_button_Click(object sender, RoutedEventArgs e)
        {
            ListViewTaskItem selectedTaskItem = (ListViewTaskItem)tasks_listView.SelectedItem;
            if (selectedTaskItem != null)
            {
                AddTaskWindow addTaskWindow = new AddTaskWindow(selectedTaskItem);
                addTaskWindow.ShowDialog();
            }
        }

        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            ListViewTaskItem selectedTaskItem = (ListViewTaskItem)tasks_listView.SelectedItem;
            if(selectedTaskItem != null)
            {
                if (selectedTaskItem.lastResult != "running...")
                {
                    AppTaskScheduler scheduler = new AppTaskScheduler();
                    
                    int result = scheduler.StartTask("ID" + selectedTaskItem.Id);
                    if(result != 267009) //task is currently running
                    { 
                        MessageBox.Show(Languages.Lang.taskNotStarted, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        selectedTaskItem.lastResult = "running...";
                    }
                }
                else
                {
                    MessageBox.Show(Languages.Lang.taskIsRunning, Languages.Lang.warning, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}