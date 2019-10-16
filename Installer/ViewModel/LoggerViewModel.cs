using Common.Utils;
using Common.Utils.Constants;
using Logger.Model;
using Logger.Utils;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System.Collections.ObjectModel;

namespace Logger.ViewModel
{
    internal class LoggerViewModel : NotifyPropertyChanged
    {
        #region Private fields
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructor
        public LoggerViewModel()
        {
            this.LoadData();
            LoggerUtils.LogMessage("Logger up and running", (LogLevel)LogLevel.Info, LoggerViewModel.logger);
        }
        #endregion

        #region Proprieties
        public ObservableCollection<LogObject> Logs
        {
            get
            {
                return LoggerUtils.logs;
            }
            set
            {
                LoggerUtils.logs = value;
                this.OnPropertyChanged(nameof(Logs));
            }
        }
        #endregion

        #region Private methods
        private void LoadData()
        {
            LoggerUtils.logs = new ObservableCollection<LogObject>();
            LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
            FileTarget fileTarget1 = new FileTarget();
            fileTarget1.FileName = Paths.LoggerFile;
            FileTarget fileTarget2 = fileTarget1;
            MemoryTarget memoryTarget = new MemoryTarget("logmemory");
            loggingConfiguration.AddRule((LogLevel)LogLevel.Info, (LogLevel)LogLevel.Fatal, (Target)fileTarget2, "*");
            loggingConfiguration.AddRule((LogLevel)LogLevel.Info, (LogLevel)LogLevel.Fatal, (Target)memoryTarget, "*");
            LogManager.Configuration = loggingConfiguration;
        }
        #endregion
    }
}