using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using VSToDoList.BL.Base;
using VSToDoList.Models;

namespace VSToDoList.UI.MainWindow
{
    public class ToDoListWindowViewModel : NotifiesPropertyChanged
    {
        public ToDoListWindowViewModel()
        {
            TasksList = new ObservableCollection<ITask>();
            var task = new Task();
            task.Name = "Parent task";
            task.SubTasks.Add(new Task() { Name = "Child Task" });
            _tasksList.Add(task);
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
                    _addNewTaskCommand = new RelayCommand<ITask>((task)=>AddNewTask(task));
                return _addNewTaskCommand;
            }
        }

        void AddNewTask(ITask parentTask)
        {
            
            var evenHandler = new PropertyChangedEventHandler(parentTask.OnChildItemPropertyChanged);
            var task = new Task();
            task.PropertyChanged += evenHandler;
            parentTask.SubTasks.Add(task);

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

        void RemoveTask(ITask taskToRemove)
        {
            var parentTask = TasksList.FirstOrDefault(x => x.SubTasks.Contains(taskToRemove));
            if(parentTask!=null)
            {
                ((Task)taskToRemove).PropertyChanged -= parentTask.OnChildItemPropertyChanged;
                parentTask.SubTasks.Remove(taskToRemove);
                return;
            }

            TasksList.Remove(taskToRemove);
        }

    }
}