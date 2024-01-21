using System;
using System.Collections.Generic;
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
using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Security.Principal;

namespace FirebirdBackupManager.Views
{
    /// <summary>
    /// Logika interakcji dla klasy GetPasswordWindow.xaml
    /// </summary>
    public partial class GetPasswordWindow : Window
    {
        public string Password { get; set; }
        public string Username { get; set; }
        public bool isResultOK { get; set; } = false;

        public GetPasswordWindow()
        {
            InitializeComponent();
            user_TextBox.Text = LoadCurrentUser();
            Username = LoadCurrentUser();
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            Password = password_PassworBox.Password;
            if (IsPasswordCorrect(Username, Password))
            {
                isResultOK = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(Languages.Lang.windowsCredentialsWindowError, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        bool IsPasswordCorrect(string userName, string password)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                return context.ValidateCredentials(userName, password);
            }
        }

        private string LoadCurrentUser()
        {
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            return currentIdentity.Name;
        }
    }
}
