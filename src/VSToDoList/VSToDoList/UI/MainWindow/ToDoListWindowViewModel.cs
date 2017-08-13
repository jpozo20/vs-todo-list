using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using VSToDoList.BL.Base;
using VSToDoList.BL.Helpers;
using VSToDoList.Models;

namespace VSToDoList.UI.MainWindow
{
    public class ToDoListWindowViewModel : NotifiesPropertyChanged
    {
        public ToDoListWindowViewModel()
        {
            TasksList = new ObservableCollection<ITask>();
        }

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
    }
}