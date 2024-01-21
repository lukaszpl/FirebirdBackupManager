using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirebirdBackupManager.Logic
{
    internal class FirebirdConnection
    {
        public static bool testConnection(string serverIP, string port, string user, string password, string databasePath)
        {
            string connectionString = $"User={user};Password={password};Database={databasePath};DataSource={serverIP};Port={port};Dialect=3;Charset=NONE;";

            using (FbConnection connection = new FbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show(Languages.Lang.succesConnectionMsg, Languages.Lang.success, MessageBoxButton.OK, MessageBoxImage.Information);
                    connection.Close();
                    return true;
                }
                catch (FbException ex)
                {
                    MessageBox.Show(ex.Message + " Code: " + ex.ErrorCode, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return false;
        }
        public static bool testConnection(string serverIP, string port, string user, string password)
        {
            string connectionString = $"User={user};Password={password};Database=employee;DataSource={serverIP};Port={port};Dialect=3;Charset=NONE;";

            using (FbConnection connection = new FbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show(Languages.Lang.succesConnectionMsg, Languages.Lang.success, MessageBoxButton.OK, MessageBoxImage.Information);
                    connection.Close();
                    return true;
                }
                catch (FbException ex)
                {
                    if (ex.ErrorCode == 335544344)
                    {
                        MessageBox.Show(Languages.Lang.succesConnectionMsg, Languages.Lang.success, MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(ex.Message + " Code: " + ex.ErrorCode, Languages.Lang.error, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return false;
        }
    }
}
