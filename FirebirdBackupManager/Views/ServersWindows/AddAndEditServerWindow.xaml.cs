using CommonClassLibrary.Models;
using FirebirdBackupManager.Logic;
using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Sockets;
using System.Security.Authentication;
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

namespace FirebirdBackupManager.Views.ServersWindows
{
    /// <summary>
    /// Logika interakcji dla klasy AddAndEditServerWindow.xaml
    /// </summary>
    public partial class AddAndEditServerWindow : Window
    {
        public AddAndEditServerWindow()
        {
            InitializeComponent();
        }

        private async void save_Button_Click(object sender, RoutedEventArgs e)
        {
            string serverName = serverName_textBox.Text;
            string serverIP = serverIP_textBox.Text;
            string serverPort = serverPort_textBox.Text;
            string user = user_textBox.Text;
            string password = password_passwordBox.Password;
            progressBar_progressBar.Visibility = Visibility.Visible;
            progressBar_progressBar.IsIndeterminate = true;
            this.IsEnabled = false;
            bool serverIsAvilibe = await Task.Run(() => testConnection(serverIP, serverPort, user, password));
            progressBar_progressBar.Visibility = Visibility.Hidden;
            this.IsEnabled = true;
            if (serverIsAvilibe)
            {
                FBServerInfo fBServerInfo = new FBServerInfo(getNewServerId(), serverName, serverIP, Convert.ToInt32(serverPort), user, password);
                GlobalListsData.fbServersList.Add(fBServerInfo);
                //save to file
                string json = JsonConvert.SerializeObject(GlobalListsData.fbServersList);
                File.WriteAllText(App.jsonDirectory + "\\FbServers.json", json);
                //
                this.Close();
            }
        }

        private int getNewServerId()
        {
            return GlobalListsData.fbServersList.Count();
        }

        private async Task<bool> testConnection(string serverIP, string serverPort, string user, string password)
        {
            bool result = FirebirdConnection.testConnection(serverIP, serverPort, user, password);
            return result;
        }
       
    }
}
