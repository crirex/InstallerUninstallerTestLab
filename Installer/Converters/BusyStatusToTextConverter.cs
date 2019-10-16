using Installer.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Installer.Converters
{
    internal sealed class BusyStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool busy = (bool)value;
                return busy ? "Build not ready" : "Ready to install";
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return default(bool);
        }
    }
}