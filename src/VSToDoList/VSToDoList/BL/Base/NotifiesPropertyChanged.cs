using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VSToDoList.BL.Base
{
    public class NotifiesPropertyChanged : INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var method = PropertyChanged;
            if (method != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}