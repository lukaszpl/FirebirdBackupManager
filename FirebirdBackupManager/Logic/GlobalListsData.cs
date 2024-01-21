using CommonClassLibrary.Models;
using CommonClassLibrary.Models.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebirdBackupManager.Logic
{
    internal class GlobalListsData
    {
        public static ObservableCollection<FBServerInfo>? fbServersList { get; set; }
        public static ObservableCollection<ListViewTaskItem>? taskItemsList { get; set; }
        public static ObservableCollection<BackupStorageInfo>? backupStorageInfoList { get; set; }
        public static List<TaskResult>? taskResults { get; set; }
    }
}
