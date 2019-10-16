using Logger.Utils;
using Microsoft.Win32;
using NLog;
using System;
using System.Windows;

namespace InstallerUninstallerTestLab.View
{
    /// <summary>
    /// Interaction logic for AppView.xaml
    /// </summary>
    public partial class AppView : Window
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        public AppView()
        {
            InitializeComponent();
            SystemEvents.SessionEnding += SessionEndingEvtHandler;
        }

        private void SessionEndingEvtHandler(object sender, SessionEndingEventArgs e)
        {
            e.Cancel = true;
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            LoggerUtils.LogMessage("Program closed.", LogLevel.Info, logger);
            Application.Current.Shutdown();
        }
    }
}
