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
using static CommonClassLibrary.Models.Storage.BackupStorageInfo;

namespace FirebirdBackupManager.Views.StorageWindows
{
    /// <summary>
    /// Logika interakcji dla klasy AddAndEditStorageWindowxaml.xaml
    /// </summary>
    public partial class AddAndEditStorageWindow : Window
    {
        public AddAndEditStorageWindow()
        {
            InitializeComponent();
        }

        private void Save_button_Click(object sender, RoutedEventArgs e)
        {
            string storageType = "Local";
            if (storageType_comboBox.SelectedIndex == 0)
                storageType = "Local";
            string storageName = name_textBox.Text;
            string storagePath = path_textBox.Text;
            if (Directory.Exists(storagePath))
            {
                BackupStorageInfo backupStorageInfo = new BackupStorageInfo(getNewStorageId(), storageName, storageType, storagePath, null, null);
                GlobalListsData.backupStorageInfoList.Add(backupStorageInfo);
                //save to file
                string json = JsonConvert.SerializeObject(GlobalListsData.backupStorageInfoList);
                File.WriteAllText(App.jsonDirectory + "\\backupStorage.json", json);
                //
                this.Close();
            }
            else
            {
                MessageBox.Show(Languages.Lang.folderNotExists, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int getNewStorageId()
        {
            return GlobalListsData.backupStorageInfoList.Count();
        }

    }
}
