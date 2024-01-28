using CommonClassLibrary.Models;
using CommonClassLibrary.Models.Storage;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace TaskWorker
{
    internal class JsonLoader
    {
        //
        public static string jsonDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FirebirdBackupManager";
        public static string backupStorageFilePath = jsonDirectory + "\\backupStorage.json";
        public static string FbServersFilePath = jsonDirectory + "\\FbServers.json";
        public static string tasksFilePath = jsonDirectory + "\\tasks.json";
        public static string resultsFilePath = jsonDirectory + "\\results.json";
        public static string notificationsSettingsFilePath = jsonDirectory + "\\notificationSettings.json";
        //
        public static List<TaskResult> taskResults { get; set; }
        public static MailConfig mailConfig { get; set; }
        public static List<TaskItem> tasks;
        public static List<FBServerInfo> Fbservers;
        public static List<BackupStorageInfo> backupStorageInfos;

        public static void SaveResult(string filePath)
        {
            string json = JsonConvert.SerializeObject(taskResults);
            File.WriteAllText(filePath, json);
        }
        public static void LoadResults()
        {
            if (File.Exists(resultsFilePath))
            {
                try
                {
                    taskResults = JsonConvert.DeserializeObject<List<TaskResult>>(File.ReadAllText(resultsFilePath));
                }catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    Environment.Exit(101); //exit with code 101 - problem with deserialize JSON
                }
            }
        }
        public static void LoadNotificationSettings() {
            if (File.Exists(notificationsSettingsFilePath))
            {
                try
                {
                    mailConfig = JsonConvert.DeserializeObject<MailConfig>(File.ReadAllText(notificationsSettingsFilePath));
                } 
                catch (Exception e){
                    Console.WriteLine($"{e.Message}");
                    Environment.Exit(101); //exit with code 101 - problem with deserialize JSON
                }
            }
        }
        public static void LoadDataFromJson()
        {
            try
            {
                Fbservers = JsonConvert.DeserializeObject<List<FBServerInfo>>(File.ReadAllText(FbServersFilePath));
                backupStorageInfos = JsonConvert.DeserializeObject<List<BackupStorageInfo>>(File.ReadAllText(backupStorageFilePath));
                tasks = JsonConvert.DeserializeObject<List<TaskItem>>(File.ReadAllText(tasksFilePath));
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                Environment.Exit(101); //exit with code 101 - problem with deserialize JSON
            }
        }

    }
}
