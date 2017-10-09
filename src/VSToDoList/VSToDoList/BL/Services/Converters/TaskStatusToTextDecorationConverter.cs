using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VSToDoList.BL.Services.Converters

{
    /// <summary>
    /// Converts between a <see cref="Models.ITask"/> Status and a <seealso cref="TextDecorationCollection"/>
    /// </summary>
    internal class TaskStatusToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Models.TaskStatus)) throw new ArgumentException("Task Status");
            if ((Models.TaskStatus)value == Models.TaskStatus.Done) return TextDecorations.Strikethrough;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Task status convert back");
        }
    }
}