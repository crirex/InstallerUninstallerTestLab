using Logger.Utils;
using NLog;
using System;
using System.IO;

namespace Installer.Utils
{
    internal static class Dialogs
    {
        public static void ElapsedTime(string message, DateTime elapsedTime, NLog.Logger logger)
        {
            LoggerUtils.LogMessage(message + " completed in " + 
                string.Format("{0} hours ", (object)elapsedTime.Hour) + 
                string.Format("{0} minutes and ", (object)elapsedTime.Minute) + 
                string.Format("{0} seconds.", (object)elapsedTime.Second), 
                (LogLevel)LogLevel.Info, logger);
        }

        public static void InstallationFailed(string path, NLog.Logger logger)
        {
            LoggerUtils.LogMessage("Installation failed for " + 
                Path.GetFileName(path) + ", path " + path + ".", 
                (LogLevel)LogLevel.Error, logger);
        }

        public static void UnknownError(string message, NLog.Logger logger)
        {
            LoggerUtils.LogMessage(message, (LogLevel)LogLevel.Error, logger);
        }

        public static void UnknownPathError(string message, NLog.Logger logger)
        {
            LoggerUtils.LogMessage(message + 
                ". Write the locations of the Testlab installers. " +
                "It should be a folder named Test.Lab that contains releases.", 
                (LogLevel)LogLevel.Error, logger);
        }

        public static void SavePathError(string path, NLog.Logger logger)
        {
            LoggerUtils.LogMessage("The path: " + 
                path + " could not be saved", 
                (LogLevel)LogLevel.Error, logger);
        }

        public static void PathModified(string path, NLog.Logger logger)
        {
            LoggerUtils.LogMessage("Current path: " + path, 
                (LogLevel)LogLevel.Info, logger);
        }

        public static void NoApplications(NLog.Logger logger)
        {
            LoggerUtils.LogMessage("There have been no applications found. " +
                "Use the search button in the Uninstaller tab to update this list.", 
                (LogLevel)LogLevel.Warn, logger);
        }

        public static void DependenciesAddedError(NLog.Logger logger)
        {
            LoggerUtils.LogMessage("The dependencies could not be saved", 
                (LogLevel)LogLevel.Error, logger);
        }

        public static void DependenciesAdded(NLog.Logger logger)
        {
            LoggerUtils.LogMessage("Dependencies were saved successfully", 
                (LogLevel)LogLevel.Info, logger);
        }

        public static void DependencyFoundError(string firstName, string secondName, NLog.Logger logger)
        {
            LoggerUtils.LogMessage("Application " + firstName + 
                " depends on application " + secondName + 
                ". You need to uninstall " + secondName + 
                " if you want to uninstall " + firstName + ".", 
                (LogLevel)LogLevel.Warn, logger);
        }
    }
}