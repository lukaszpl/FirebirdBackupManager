using CommonClassLibrary;
using CommonClassLibrary.Models;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace FirebirdBackupManager.Views
{
    /// <summary>
    /// Logika interakcji dla klasy NotificationsSettingsWindow.xaml
    /// </summary>
    public partial class NotificationsSettingsWindow : Window
    {
        public NotificationsSettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private async void save_button_Click(object sender, RoutedEventArgs e)
        {
            //
            Int32.TryParse(port_TextBox.Text, out int port);
            string server = Server_textBox.Text; string username = username_textBox.Text; string password = password_passwordBox.Password; bool? SSL = SSL_checkBox.IsChecked; string receiver = reciver_textBox.Text;
            //
            this.IsEnabled = false;
            progressbar_Progressbar.Visibility = Visibility.Visible;
            progressbar_Progressbar.IsIndeterminate = true;
            string result = await Task.Run(() => sendEmail(server, port, username, password, SSL, receiver, "Firebird backup manager", "TEST EMAIL"));
            this.IsEnabled = true;
            progressbar_Progressbar.Visibility = Visibility.Hidden;
            if (!result.Equals("true"))
            {
                MessageBox.Show(result, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                SaveSettings(Server_textBox.Text, port.ToString(), username_textBox.Text, password_passwordBox.Password, SSL_checkBox.IsChecked, reciver_textBox.Text);
                MessageBox.Show(Languages.Lang.sendMailInfo, Languages.Lang.success, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveSettings(string smtpServer, string smtpPort, string smtpUserName, string smtpPassword, bool? SSL, string reciver)
        {
            MailConfig mailConfig = new MailConfig(smtpServer, smtpPort, smtpUserName, smtpPassword, SSL, reciver);
            string json = JsonConvert.SerializeObject(mailConfig);
            File.WriteAllText(App.jsonDirectory + "\\notificationSettings.json", json);
            this.Close();
        }

        private void LoadSettings()
        {
            try
            {
                MailConfig mailConfig = JsonConvert.DeserializeObject<MailConfig>(File.ReadAllText(App.jsonDirectory + "\\notificationSettings.json"));
                Server_textBox.Text = mailConfig.smtpServer;
                port_TextBox.Text = mailConfig.smtpPort;
                username_textBox.Text = mailConfig.smtpUserName;
                password_passwordBox.Password = mailConfig.smtpPassword;
                reciver_textBox.Text = mailConfig.receiver;
            }catch (Exception ex) { }
        }

        private async Task<string> sendEmail(string server, int port, string username, string password, bool? SSL, string receiver, string subject, string content)
        {
            return MailSender.SendEmail(server, port, username, password, SSL, receiver, subject, content);
        }
    }
}
