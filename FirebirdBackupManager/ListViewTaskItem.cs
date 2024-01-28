using CommonClassLibrary.Models;
using CommonClassLibrary.Models.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FirebirdBackupManager
{
    public class ListViewTaskItem : TaskItem, INotifyPropertyChanged
    {

        public string? serverIP { get; set; }
        private string? LastActive;
        public string? lastActive
        {
            get { return LastActive; }
            set
            {
                if (LastActive != value)
                {
                    LastActive = value;
                    OnPropertyChanged(nameof(LastActive));
                }
            } 
        }
        private string? LastResult;
        public string? lastResult
        {
            get { return LastResult; }
            set
            {
                if (LastResult != value)
                {
                    LastResult = value;
                    OnPropertyChanged(nameof(LastResult));
                }
            }
        }

        private string? ScheduledAt;
        public string? scheduledAt 
        { 
            get
            {
                if(scheduleType == ScheduleType.Everyday)
                {
                    return "Everyday at " + timeHour.ToString();
                }
                else
                {
                    return scheduleType.ToString() + " " + timeHour.ToString();
                }
            }
            set {OnPropertyChanged(nameof(ScheduledAt)); }
        }

        private string? Comment;
        public string? comment
        {
            get { return Comment; }
            set { Comment = value; OnPropertyChanged(nameof(Comment));}
        }
        private string? DatabasePath;
        public string? databasePath
        {
            get { return DatabasePath; }
            set { DatabasePath = value; OnPropertyChanged(nameof(DatabasePath));}
        }
        public ListViewTaskItem(int id, string? taskName, DateTime? createdAt, string lastResult, string lastActive, string? comment, ScheduleType scheduleType, string timeHour, int retentionTime, int fBServerId, int storageId, string databasePath, string serverIP) : base(id, taskName, createdAt, comment, scheduleType, timeHour, retentionTime, databasePath, fBServerId, storageId)
        {
            this.serverIP = serverIP;
            this.lastActive = lastActive;
            this.lastResult = lastResult;
            this.Comment = comment;
            this.DatabasePath = databasePath;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
