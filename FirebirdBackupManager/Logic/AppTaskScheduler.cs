using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdBackupManager.Views;
using Microsoft.Win32.TaskScheduler;

namespace FirebirdBackupManager.Logic
{
    internal class AppTaskScheduler
    {
        string taskFolderName = "FirebirdTaskManager";
        int hour; int minutes;
        public bool CreateTask(string taskName, string time, string pathToApp, string parameters)
        {
            ConvertTimeToInts(time);
            
            using (TaskService taskService = new TaskService())
            {         
                //
                TaskFolder rootFolder = taskService.RootFolder;
                TaskFolder taskFolder;
                if (!rootFolder.SubFolders.Exists(taskFolderName))
                {
                    taskFolder = rootFolder.CreateFolder(taskFolderName);
                }
                else
                {
                    taskFolder = rootFolder.SubFolders[taskFolderName];
                }
                TaskDefinition taskDefinition = TaskService.Instance.NewTask();
                taskDefinition.RegistrationInfo.Description = "FirebirdBackupManagerTask";
               
                DailyTrigger dailyTrigger = new DailyTrigger();
                dailyTrigger.StartBoundary = DateTime.Today.AddHours(hour).AddMinutes(minutes);
                dailyTrigger.DaysInterval = 1;
                dailyTrigger.Enabled = true;
                taskDefinition.Triggers.Add(dailyTrigger);
                taskDefinition.Actions.Add(pathToApp, parameters);
                taskDefinition.Principal.LogonType = TaskLogonType.Password;
                taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;

                //get windows credentials
                GetPasswordWindow getPasswordWindow = new GetPasswordWindow();
                getPasswordWindow.ShowDialog();
                if(getPasswordWindow.isResultOK == true)
                {
                    taskFolder.RegisterTaskDefinition(taskName, taskDefinition, TaskCreation.CreateOrUpdate, getPasswordWindow.Username, getPasswordWindow.Password);
                    return true;
                }
                else
                {
                    return false;
                }
                //            
            } 
        }

        public void DeleteTask(string taskName)
        {
            using (TaskService taskService = new TaskService())
            {
                TaskFolder rootFolder = taskService.RootFolder;
                TaskFolder taskFolder;
                if (rootFolder.SubFolders.Exists(taskFolderName))
                {
                    taskFolder = rootFolder.SubFolders[taskFolderName];
                    foreach(var task in taskFolder.GetTasks())
                    {
                        if (task.Name == taskName)
                            taskFolder.DeleteTask(taskName);
                    }
                }
            }
        }

        public int StartTask(string taskName)
        {
            using (TaskService taskService = new TaskService())
            {
                TaskFolder rootFolder = taskService.RootFolder;
                TaskFolder taskFolder;
                if (rootFolder.SubFolders.Exists(taskFolderName))
                {
                    taskFolder = rootFolder.SubFolders[taskFolderName];
                    foreach (var task in taskFolder.GetTasks())
                    {
                        if (task.Name == taskName)
                            return task.Run().LastTaskResult;
                    }
                }              
            }
            return -1;
        }
    

    private void ConvertTimeToInts(string time)
        {
            string[] timeArray = time.Split(':');
            if (timeArray.Length == 2 && int.TryParse(timeArray[0], out int hour) && int.TryParse(timeArray[1], out int minutes))
            {
                if (IsHourValid(hour) && IsMinuteValid(minutes))
                {
                    this.hour = hour;
                    this.minutes = minutes;
                }
                else
                {
                    throw new FormatException("Time is not valid!");
                }
            }
        }

        private bool IsHourValid(int hour)
        {
            return hour >= 0 && hour <= 23;
        }

        private bool IsMinuteValid(int minute)
        {
            return minute >= 0 && minute <= 59;
        }
    }
}
