using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using VSToDoList.BL.Base;
using VSToDoList.BL.Helpers;
using VSToDoList.BL.Services.TaskServices;
using VSToDoList.Models;

namespace VSToDoList.UI.MainWindow.ViewModels
{
    public class ToDoListWindowViewModel : NotifiesPropertyChanged
    {
        public ToDoListWindowViewModel()
        {
            TasksList = new ObservableCollection<ITask>();
            _taskService = new TaskService();
            _solutionEventsListener = new SolutionEventsListener
            {
                SolutionClosingCallback = SaveTasks,
                SolutionOpenedCallback = LoadTasks
            };

            // Attempt to load the solution tasks when the tool window
            // wasn't active at VS startup, otherwise just use the event listener
            LoadTasks();
            StartListeningToSolutionEvents();
        }

        private void StartListeningToSolutionEvents()
        {
            IVsSolution solution = ApplicationCommons.Services.GetVsSolutionService();
            if (solution == null) return;

            solution.AdviseSolutionEvents(_solutionEventsListener, out uint cookie);
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
                    _addNewTaskCommand = new RelayCommand<ITask>(task => AddNewTask(task));
                return _addNewTaskCommand;
            }
        }

        private void AddNewTask(ITask parentTask)
        {
            Task task = new Task();

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
            ITask parentTask = TaskHelper.FindParentTask(TasksList, taskToRemove);
            if (parentTask != null)
            {
                parentTask.SubTasks.Remove(taskToRemove);
                return;
            }

            TasksList.Remove(taskToRemove);
        }

        /// <summary>
        /// Moves a task from a position into another position. It's used for drag and drop.
        /// </summary>
        /// <param name="current">The source task</param>
        /// <param name="after">The target task</param>
        public void MoveTask(ITask current, ITask after)
        {
            ITask currentParent = TaskHelper.FindParentTask(_tasksList, current);
            ITask afterParent = TaskHelper.FindParentTask(_tasksList, after);
            Move(currentParent, current, afterParent, after);
        }

        private void Move(ITask parentOfCurrent, ITask current, ITask parentOfAfter, ITask after)
        {
            if (parentOfCurrent == parentOfAfter)
            {
                // If they're both null it means neither of them has a parent
                if (parentOfCurrent == null) TaskHelper.SwapItems(_tasksList, after, current);
                else TaskHelper.SwapItems(parentOfCurrent.SubTasks, after, current);
            }
            else if (parentOfCurrent != null && parentOfAfter == null)
            {
                int afterIndex = _tasksList.IndexOf(after);
                _tasksList.Insert(afterIndex, current);
                parentOfCurrent.SubTasks.Remove(current);
            }
            else if (parentOfCurrent == null && parentOfAfter != null)
            {
                int afterIndex = parentOfAfter.SubTasks.IndexOf(after);
                parentOfAfter.SubTasks.Insert(afterIndex, current);
                _tasksList.Remove(current);
            }
            else
            {
                int afterIndex = parentOfAfter.SubTasks.IndexOf(after);
                parentOfAfter.SubTasks.Insert(afterIndex, current);
                parentOfCurrent.SubTasks.Remove(current);
            }
        }

        /// <summary>
        /// Gets the solution name and saves the tasks related to that solution
        /// Data is saved to {SolutionRoot}/{solutionname}.tasks
        /// </summary>
        private void SaveTasks()
        {
            string solutionFullName = GetSolutionFullName();
            if (string.IsNullOrWhiteSpace(solutionFullName)) return;

            string solutionName = Path.GetFileNameWithoutExtension(solutionFullName);
            string solutionFolderPath = Path.GetDirectoryName(solutionFullName);
            if (string.IsNullOrWhiteSpace(solutionFullName) || string.IsNullOrWhiteSpace(solutionFolderPath)) return;

            _taskService.SaveTasks(solutionName, solutionFolderPath, TasksList.ToList());
            TasksList.Clear();
        }

        /// <summary>
        /// Gets the solution name and loads the tasks related to that solution
        /// Data is loaded from {SolutionRoot}/{solutionname}.tasks
        /// </summary>
        private void LoadTasks()
        {
            string solutionFullName = GetSolutionFullName();
            if (string.IsNullOrWhiteSpace(solutionFullName)) return;

            string solutionName = Path.GetFileNameWithoutExtension(solutionFullName);
            string solutionFolderPath = Path.GetDirectoryName(solutionFullName);
            if (string.IsNullOrWhiteSpace(solutionFullName) || string.IsNullOrWhiteSpace(solutionFolderPath)) return;

            System.Collections.Generic.ICollection<ITask> tasks = _taskService.LoadTasks(solutionName, solutionFolderPath);
            if (tasks != null && tasks.Count > 0)
            {
                foreach (ITask task in tasks)
                {
                    TasksList.Add(task);
                }
            }
        }

        /// <summary>
        /// Uses the EnvDTE service to get the name of the currently loaded solution
        /// </summary>
        /// <returns></returns>
        private string GetSolutionFullName()
        {
            _dte = ApplicationCommons.Services.GetEnvDTE();
            if (_dte == null) return string.Empty;

            string solutionFullName = _dte.Solution.FullName;
            if (string.IsNullOrWhiteSpace(solutionFullName)) return string.Empty;
            return solutionFullName;
        }
    }
}