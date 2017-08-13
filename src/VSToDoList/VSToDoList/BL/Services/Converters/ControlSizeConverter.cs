using System;
using System.Windows.Data;

namespace VSToDoList.BL.Services.Converters
{
    public class ControlSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value >= 25)
            {
                return (double)value - 25;
            }
            return (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members

        public double LeftMargin { get; set; }
        public double RightMargin { get; set; }
    }
}