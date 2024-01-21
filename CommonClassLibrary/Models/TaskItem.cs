using CommonClassLibrary.Models.Storage;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary.Models
{
    public class TaskItem
    {
        public TaskItem(int id, string? taskName, DateTime? createdAt, string? comment, ScheduleType scheduleType, string timeHour, int retentionTime, string? databasePath, int fBServerId, int storageId)
        {
            Id = id;
            this.fBServerId = fBServerId;
            this.taskName = taskName;
            this.createdAt = createdAt;
            this.comment = comment;
            this.scheduleType = scheduleType;
            this.timeHour = timeHour;
            this.databasePath = databasePath;
            this.storageId = storageId;
            this.retentionTime = retentionTime;
        }

        public int Id { get; set; }
        public string? taskName { get; set; }
        public DateTime? createdAt { get; set; }
        public string? comment { get; set; }
        public ScheduleType scheduleType { get; set; }
        public string timeHour {  get; set; }
        public int retentionTime {get; set; }
        public string? databasePath { get; set; }
        public int fBServerId { get; set; }
        public int storageId { get; set; }       
    }
    public enum ScheduleType { Everyday, Weekly }
}
