using CommonClassLibrary.Models;
using CommonClassLibrary.Models.Storage;
using FirebirdBackupManager.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FirebirdBackupManager.Views.StorageWindows
{
    /// <summary>
    /// Logika interakcji dla klasy StoragesWindow.xaml
    /// </summary>
    public partial class StorageListWindow : Window
    {
        public StorageListWindow()
        {
            InitializeComponent();
            storage_ListView.ItemsSource = GlobalListsData.backupStorageInfoList;
        }

        private void Add_button_Click(object sender, RoutedEventArgs e)
        {
            AddAndEditStorageWindow addAndEditStorageWindow = new AddAndEditStorageWindow();
            addAndEditStorageWindow.ShowDialog();
        }

        private void Delete_button_Click(object sender, RoutedEventArgs e)
        {
            BackupStorageInfo selectedItem = (BackupStorageInfo)storage_ListView.SelectedItem;
            if ((GlobalListsData.taskItemsList != null) && (selectedItem != null))
            {
                foreach (var task in GlobalListsData.taskItemsList)
                {
                    //if task is correlated with storage, don't remove it
                    if (task.storageId == selectedItem.id)
                    {
                        MessageBox.Show(Languages.Lang.ErrorDeleteStorageWhenTasksAreCorrelated, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            GlobalListsData.backupStorageInfoList.Remove(selectedItem);
            string json = JsonConvert.SerializeObject(GlobalListsData.backupStorageInfoList);
            File.WriteAllText(App.jsonDirectory + "\\backupStorage.json", json);
        }
    }
}
