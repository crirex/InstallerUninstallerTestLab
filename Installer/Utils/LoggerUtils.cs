using Installer.Utils;
using Logger.Model;
using NLog;
using NLog.Targets;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Logger.Utils
{
    internal static class LoggerUtils
    {
        public static ObservableCollection<LogObject> logs = new ObservableCollection<LogObject>();

        public static bool IsLoggerLocationVisible { get; set; } = ConfigLoader.Instance.ApplicationConfig.isLoggerLocationVisible;

        public static ObservableCollection<LogObject> GetLogsAsLogObject()
        {
            ObservableCollection<LogObject> observableCollection = new ObservableCollection<LogObject>();
 
            foreach (string log in (IEnumerable<string>)((MemoryTarget)LogManager.Configuration.FindTargetByName<MemoryTarget>("logmemory")).Logs)
                observableCollection.Add(LoggerUtils.StringToLogObject(log));
            return observableCollection;
        }

        private static LogObject GetLastLogAsLogObject()
        {
            try
            {
                return LoggerUtils.StringToLogObject(((MemoryTarget)LogManager.Configuration.FindTargetByName<MemoryTarget>("logmemory")).Logs.Last<string>());
            }
            catch
            {
                return new LogObject();
            }
        }

        private static LogObject StringToLogObject(string log)
        {
            string[] strArray = log.Split('|');
            return new LogObject()
            {
                Date = strArray[0],
                Type = strArray[1],
                Location = strArray[2],
                Message = strArray[3]
            };
        }

        public static void LogMessage(string message, LogLevel level, NLog.Logger logger)
        {
            logger.Log(level, message);
            LoggerUtils.logs.Add(LoggerUtils.GetLastLogAsLogObject());
        }
    }
}