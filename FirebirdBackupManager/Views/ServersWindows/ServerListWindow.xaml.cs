using CommonClassLibrary.Models;
using FirebirdBackupManager.Logic;
using FirebirdBackupManager.Views.ServersWindows;
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

namespace FirebirdBackupManager.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ServerListWindow.xaml
    /// </summary>
    public partial class ServerListWindow : Window
    {
        public ServerListWindow()
        {
            InitializeComponent();
            FbServers_listView.ItemsSource = GlobalListsData.fbServersList;
        }

        private void AddServer_button_Click(object sender, RoutedEventArgs e)
        {
            AddAndEditServerWindow addAndEditServerWindow = new AddAndEditServerWindow();
            addAndEditServerWindow.ShowDialog();
        }

        private void DeleteServer_button_Click(object sender, RoutedEventArgs e)
        {
            FBServerInfo selectedItem = (FBServerInfo)FbServers_listView.SelectedItem;
            if ((GlobalListsData.taskItemsList != null) && (selectedItem!=null))
            {
                foreach (var task in GlobalListsData.taskItemsList)
                {
                    //if task is correlated with server, don't remove it
                    if(task.fBServerId == selectedItem.id)
                    {
                        MessageBox.Show(Languages.Lang.ErrorDeleteFbServerWhenTasksAreCorrelated, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            GlobalListsData.fbServersList.Remove(selectedItem);
            string json = JsonConvert.SerializeObject(GlobalListsData.fbServersList);
            File.WriteAllText(App.jsonDirectory + "\\FbServers.json", json);
        }
    }
}
