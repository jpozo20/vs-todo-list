using System.Collections.ObjectModel;
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
    }
}