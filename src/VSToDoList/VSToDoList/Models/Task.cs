using System.Collections.ObjectModel;
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
    }

    public class Task : NotifiesPropertyChanged, ITask
    {
        public Task()
        {
            SubTasks = new ObservableCollection<ITask>();
        }

        public string Name { get; set; }

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
        public bool IsExpanded { get; set; }

        public ObservableCollection<ITask> SubTasks { get; set; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (string.Equals(propertyName, nameof(IsDone)))
            {
            }
            base.OnPropertyChanged(propertyName);
        }
    }
}