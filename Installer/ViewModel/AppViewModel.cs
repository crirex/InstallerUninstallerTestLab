using Common.Utils;
using InstallerUninstallerTestLab.View;
using NLog;
using System;
using System.Windows;

namespace InstallerUninstallerTestLab.ViewModel
{
    internal class AppViewModel : NotifyPropertyChanged
    {
        #region Private fields
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        public AppViewModel()
        {
            this.LoadData();
        }
        #endregion

        #region Commands
        private RelayCommand exitCommand;
        public RelayCommand ExitCommand
        {
            get
            {
                if (this.exitCommand == null)
                    this.exitCommand = new RelayCommand((Action<object>)(param => this.Exit()));
                return this.exitCommand;
            }
        }

        private RelayCommand settingsCommand;
        public RelayCommand SettingsCommand
        {
            get
            {
                if (this.settingsCommand == null)
                    this.settingsCommand = new RelayCommand((Action<object>)(param => this.Settings()));
                return this.settingsCommand;
            }
        }

        private RelayCommand aboutCommand;
        public RelayCommand AboutCommand
        {
            get
            {
                if (this.aboutCommand == null)
                    this.aboutCommand = new RelayCommand((Action<object>)(param => this.About()));
                return this.aboutCommand;
            }
        }
        #endregion

        #region Private methods
        private void LoadData()
        {
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void Settings()
        {
            new SettingsView().ShowDialog();
        }

        private void About()
        {
            UserDialog.MessageDialog("Application for the installing and uninstalling of TestLab applications." +
                "\nMade by Anton Gabriel and Bălan Cristian.", DialogType.Message);
        }
        #endregion
    }
}
