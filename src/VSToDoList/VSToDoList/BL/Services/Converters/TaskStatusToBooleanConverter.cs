using System;
using System.Globalization;
using System.Windows.Data;
using VSToDoList.Models;

namespace VSToDoList.BL.Services.Converters
{
    internal class TaskStatusToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value;
            switch ((TaskStatus)status)
            {
                case TaskStatus.Done:
                    {
                        return true;
                    }
                case TaskStatus.NotDone:
                    {
                        return false;
                    }
                case TaskStatus.SemiDone:
                    {
                        return null;
                    }

                default: return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return TaskStatus.SemiDone;
            if ((bool)value == true) return TaskStatus.Done;
            if ((bool)value == false) return TaskStatus.NotDone;

            return TaskStatus.NotDone;
        }
    }
}