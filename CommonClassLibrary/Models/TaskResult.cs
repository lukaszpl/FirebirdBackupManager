using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary.Models
{
    public class TaskResult
    {
        public TaskResult(int taskId, DateTime? lastActive, string? lastResult, string? status)
        {
            this.taskId = taskId;
            this.lastActive = lastActive;
            this.lastResult = lastResult;
            this.status = status;
        }

        public int taskId { get; set; }
        public DateTime? lastActive { get; set; }
        public string? lastResult { get; set; }
        public string? status { get; set; }
    }
}
