using System;
using System.Globalization;
using System.Windows.Data;

namespace Installer.Converters
{
    internal sealed class DateTimeToDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime dateTime = (DateTime)value;
                return $"{dateTime.ToString("MMMM dd yyyy")}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.Now;
        }
    }
}