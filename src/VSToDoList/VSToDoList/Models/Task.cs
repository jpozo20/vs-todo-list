using System.Collections.ObjectModel;
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
    }

    public class Task : NotifiesPropertyChanged, ITask
    {
        public Task()
        {
            SubTasks = new ObservableCollection<ITask>();
            SubTasks.CollectionChanged += OnSubTaskItemChanged;
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

        private ObservableCollection<ITask> _subTasks;

        public ObservableCollection<ITask> SubTasks
        {
            get
            {
                return _subTasks;
            }
            set
            {
                _subTasks = value;
                OnPropertyChanged();
            }
        }

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
            }
            base.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Will need it later when adding the minus to tasks Semi-done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSubTaskItemChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// When a task is Semi-done, will put a minus sign on its CheckBox
        /// </summary>
        private void UpdateTaskStatus()
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