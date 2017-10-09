using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using VSToDoList.BL.Base;
using VSToDoList.BL.Helpers;
using VSToDoList.BL.Services.TaskServices;
using VSToDoList.Models;

namespace VSToDoList.UI.MainWindow
{
    public class ToDoListWindowViewModel : NotifiesPropertyChanged
    {

        public ToDoListWindowViewModel()
        {
            TasksList = new ObservableCollection<ITask>();
            _taskService = new TaskService();
            _solutionEventsListener = new SolutionEventsListener();
            _solutionEventsListener.SolutionClosingCallback = SaveTasks;
            _solutionEventsListener.SolutionOpenedCallback = LoadTasks;

            StartListeningToSolutionEvents();
        }

        void StartListeningToSolutionEvents()
        {
            var solution = ApplicationCommons.Services.GetVsSolutionService();
            if (solution == null) return;

            uint cookie;
            solution.AdviseSolutionEvents(_solutionEventsListener,out cookie);
            ApplicationCommons.Instances.SolutionServiceCookie = cookie;
            ApplicationCommons.Instances.SolutionService = solution;
        }
        private EnvDTE.DTE _dte;
        private readonly ITaskService _taskService;
        private readonly ISolutionEventsListener _solutionEventsListener;

        private ObservableCollection<ITask> _tasksList;

        public ObservableCollection<ITask> TasksList
        {
            get { return _tasksList; }
            set
            {
                _tasksList = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<ITask> _addNewTaskCommand;

        public RelayCommand<ITask> AddNewTaskCommand
        {
            get
            {
                if (_addNewTaskCommand == null)
                    _addNewTaskCommand = new RelayCommand<ITask>((task) => AddNewTask(task));
                return _addNewTaskCommand;
            }
        }

        private void AddNewTask(ITask parentTask)
        {
            var task = new Task();

            if (parentTask != null)
            {
                parentTask.SubTasks.Add(task);
            }
            else
            {
                TasksList.Add(task);
            }
        }

        private RelayCommand<ITask> _removeTaskCommand;

        public RelayCommand<ITask> RemoveTaskCommand
        {
            get
            {
                if (_removeTaskCommand == null)
                    _removeTaskCommand = new RelayCommand<ITask>((task) => RemoveTask(task));
                return _removeTaskCommand;
            }
        }

        private void RemoveTask(ITask taskToRemove)
        {
            var parentTask = TaskHelper.FindParentTask(TasksList, taskToRemove);
            if (parentTask != null)
            {
                parentTask.SubTasks.Remove(taskToRemove);
                return;
            }

            TasksList.Remove(taskToRemove);
        }

        /// <summary>
        /// Gets the solution name and saves the tasks related to that solution
        /// Data is saved to %AppData%/VsToDoList/{solutionname}.tasks
        /// </summary>
        private void SaveTasks()
        {
            var solutionName = GetSolutionName();
            if (solutionName == null) return;

            _taskService.SaveTasks(solutionName, TasksList.ToList());
            TasksList.Clear();
        }

        /// <summary>
        /// Gets the solution name and loads the tasks related to that solution
        /// Data is loaded from %AppData%/VsToDoList/{solutionname}.tasks
        /// </summary>
        private void LoadTasks()
        {
            var solutionName = GetSolutionName();
            if (string.IsNullOrWhiteSpace(solutionName)) return;

            var tasks = _taskService.LoadTasks(solutionName);
            if (tasks != null && tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    TasksList.Add(task);
                }
            }
        }

        /// <summary>
        /// Uses the EnvDTE service to get the name of the currently loaded solution
        /// </summary>
        /// <returns></returns>
        string GetSolutionName()
        {
            _dte = ApplicationCommons.Services.GetEnvDTE();
            if (_dte == null) return string.Empty;

            var solutionPath = _dte.Solution.FileName;
            if (string.IsNullOrWhiteSpace(solutionPath)) return string.Empty;

            var solutionName = Path.GetFileNameWithoutExtension(solutionPath);
            return solutionName;
        }
    }
}