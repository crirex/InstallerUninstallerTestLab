using Logger.Utils;
using System.Windows;
using System.Windows.Controls;

namespace Logger.View
{
    public partial class LoggerView : UserControl
    {
        public LoggerView()
        {
            this.InitializeComponent();
            if (LoggerUtils.IsLoggerLocationVisible)
                this.LocationColumn.Visibility = Visibility.Visible;
            else
                this.LocationColumn.Visibility = Visibility.Hidden;
        }
    }
}
