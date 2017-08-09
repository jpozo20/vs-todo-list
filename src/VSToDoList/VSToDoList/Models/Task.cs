using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VSToDoList.BL.Base;

namespace VSToDoList.Models
{
    public interface ITask
    {
        string Name { get; set; }
        bool IsDone { get; set; }
        bool IsExpanded { get; set; }
        ObservableCollection<ITask> SubTasks { get; set; }
        TaskStatus Status { get; set; }
        void OnChildItemPropertyChanged(object sender, PropertyChangedEventArgs e);
    }

    public class Task : NotifiesPropertyChanged, ITask
    {
        public Task()
        {
            SubTasks = new ObservableCollection<ITask>();
        }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value; OnPropertyChanged();
            }
        }

        private bool _isDone;

        public bool IsDone
        {
            get { return _isDone; }
            set
            {
                _isDone = value;
                OnPropertyChanged();
            }
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ITask> SubTasks { get; set; }

        private TaskStatus _taskStatus;
        public TaskStatus Status
        {
            get { return _taskStatus; }
            set { _taskStatus = value; OnPropertyChanged(); }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (string.Equals(propertyName, nameof(IsDone)))
            {
            }

            if (string.Equals(propertyName, nameof(SubTasks)))
            {
                UpdateTaskStatus();
            }
            base.OnPropertyChanged(propertyName);
        }

        public void OnChildItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        void UpdateTaskStatus()
        {
            if (SubTasks.Any(x => x.IsDone) && SubTasks.Any(x => !x.IsDone))
            {
                this.Status = TaskStatus.SemiDone;
            }
            else
            {
                if (SubTasks.All(x => x.IsDone)) this.Status = TaskStatus.Done;
                if (SubTasks.All(x => !x.IsDone)) this.Status = TaskStatus.NotDone;
            }
        }
    }
}