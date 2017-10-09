using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VSToDoList.BL.Base;

namespace VSToDoList.Models
{
    public interface ITask
    {
        string Name { get; set; }
        bool? IsDone { get; set; }
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
            IsDone = false;
        }

        /// <summary>
        /// Flag to avoid <see cref="System.StackOverflowException"/> when parentTask status changes
        /// Since all the subtasks statuses are changed and the PropertyChanged fires,
        /// trying to update the parent
        /// </summary>
        private bool _isParentChangedByUser = false;

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private bool? _isDone;

        public bool? IsDone
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
            set
            {
                _taskStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Update child tasks when the Status of a task changes
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (string.Equals(propertyName, nameof(Status)))
            {
                _isParentChangedByUser = true;

                if (Status == TaskStatus.Done)
                {
                    foreach (var task in SubTasks)
                    {
                        task.Status = TaskStatus.Done;
                    }
                }
                else if (Status == TaskStatus.NotDone)
                {
                    foreach (var task in SubTasks)
                    {
                        task.Status = TaskStatus.NotDone;
                    }
                }
                _isParentChangedByUser = false;
            }
            base.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Will need it later when adding the minus to tasks Semi-done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSubTaskItemChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count > 0)
                {
                    var newTask = e.NewItems[0] as Task;
                    newTask.PropertyChanged += OnChildItemPropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // For now only one item will be deleted at once
                if (e.OldItems.Count > 0)
                {
                    var oldTask = e.OldItems[0] as Task;
                    oldTask.PropertyChanged -= OnChildItemPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Called when a task in the SubTasks list changes its Status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnChildItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(Status)))
            {
                if (_isParentChangedByUser) return;
                UpdateTaskStatus();
            }
        }

        /// <summary>
        /// Update the Status of a Task when one of its Substaks changes its Status
        /// When a task is Semi-done, will put a minus sign on its CheckBox
        /// EDIT: Will use the default windows style that puts a little box in the checkbox
        /// </summary>
        private void UpdateTaskStatus()
        {
            //If at least one subtask is Done, then the task is semidone
            if (SubTasks.Any(task => task.Status == TaskStatus.Done) && SubTasks.Any(task => task.Status != TaskStatus.Done))
            {
                this.Status = TaskStatus.SemiDone;
            }
            else
            {
                if (SubTasks.All(task => task.Status == TaskStatus.Done))
                {
                    this.Status = TaskStatus.Done;
                };
                if (SubTasks.All(task => task.Status == TaskStatus.NotDone))
                {
                    this.Status = TaskStatus.NotDone;
                }
                if (SubTasks.All(task => task.Status == TaskStatus.SemiDone))
                {
                    this.Status = TaskStatus.SemiDone;
                }
            }
        }
    }
}