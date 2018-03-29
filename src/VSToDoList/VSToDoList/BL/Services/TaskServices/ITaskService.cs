﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace VSToDoList.BL.Services.TaskServices
{
    public interface ITaskService
    {
        void SaveTasks(string solutionName, string solutionFolderPath, ICollection<Models.ITask> tasks);

        ICollection<Models.ITask> LoadTasks(string solutionName, string solutionFolderPath);
    }

    public class TaskService : ITaskService
    {
        private const string FolderName = "VSToDoList";
        private const string Extension = ".tasks";

        public ICollection<Models.ITask> LoadTasks(string solutionName, string solutionFolderPath)
        {
            ICollection<Models.ITask> tasks = new List<Models.ITask>();
            try
            {
                var path = GetFinalJsonPath(solutionName, solutionFolderPath);
                var json = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    tasks = JsonConvert.DeserializeObject<ICollection<Models.ITask>>(json);
                }

                return tasks;
            }
            catch (Exception ex)
            {
                //TODO: Act according to the exception being thrown
                return tasks;
            }
        }

        void ITaskService.SaveTasks(string solutionName, string solutionFolderPath, ICollection<Models.ITask> tasks)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(tasks);
            var path = GetFinalJsonPath(solutionName, solutionFolderPath);
            try
            {
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                //TODO: Act according to the exception being thrown
            }
        }

        private string GetFinalJsonPath(string solutionName, string solutionFolderPath)
        {
            var solutionTasksPath = Path.Combine(solutionFolderPath, solutionName + Extension);
            return solutionTasksPath;
        }
    }
}