using Commmon.Utils.Writers;
using Common.Utils;
using Common.Utils.Constants;
using Installer.Actions;
using Installer.Model;
using Installer.Utils;
using InstallerUninstallerTestLab.View;
using NLog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace InstallerUninstallerTestLab.ViewModel
{
    internal class SettingsViewModel : NotifyPropertyChanged
    {
        #region Private fields
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();   
        #endregion

        #region Constructor
        public SettingsViewModel()
        {
            this.LoadData();
        }
        #endregion

        #region Commands
        private RelayCommand<Window> cancelCommand;
        public RelayCommand<Window> CancelCommand
        {
            get
            {
                if (this.cancelCommand == null)
                    this.cancelCommand = new RelayCommand<Window>((Action<Window>)(param => this.Cancel(param)));
                return this.cancelCommand;
            }
        }

        private RelayCommand<System.Windows.Controls.TextBox> browseCommand;
        public RelayCommand<System.Windows.Controls.TextBox> BrowseCommand
        {
            get
            {
                if (this.browseCommand == null)
                    this.browseCommand = new RelayCommand<System.Windows.Controls.TextBox>((Action<System.Windows.Controls.TextBox>)(param => this.Browse(param)));
                return this.browseCommand;
            }
        }

        private RelayCommand dependenciesCommand;
        public RelayCommand DependenciesCommand
        {
            get
            {
                if (this.dependenciesCommand == null)
                    this.dependenciesCommand = new RelayCommand((Action<object>)(param => this.OpenDependencies()));
                return this.dependenciesCommand;
            }
        }


        private RelayCommand<System.Windows.Controls.TextBox> savePathCommand;
        public RelayCommand<System.Windows.Controls.TextBox> SavePathCommand
        {
            get
            {
                if (this.savePathCommand == null)
                    this.savePathCommand = new RelayCommand<System.Windows.Controls.TextBox>(new Predicate<System.Windows.Controls.TextBox>(this.CanSavePath), (Action<System.Windows.Controls.TextBox>)(param => this.SavePath(param)));
                return this.savePathCommand;
            }
        }
        #endregion

        #region Proprieties
        public string UserPath { get; set; }

        public bool isCheckedLocation { get; set; }
        #endregion

        #region Private methods
        private void LoadData()
        {
        }

        private void Cancel(Window window)
        {
            window.Close();
        }

        private void Browse(System.Windows.Controls.TextBox browseTextBox)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog.SelectedPath = this.UserPath;
            folderBrowserDialog.Description = "Select the Test.Lab folder with the installers";
            folderBrowserDialog.ShowNewFolderButton = false;
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            browseTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void OpenDependencies()
        {
            new DependenciesView().ShowDialog();
        }

        private bool CanSavePath(System.Windows.Controls.TextBox pathTextBox)
        {
            return pathTextBox == null || (uint)pathTextBox.Text.Length > 0U;
        }

        private void SavePath(System.Windows.Controls.TextBox pathTextBox)
        {
            if (System.Windows.MessageBox.Show("Main window will restart. Be sure there is nothing running in the main window or it will stop.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                return;
            if (!File.Exists((string)Paths.ConfigFile))
            {
                using (StreamWriter streamWriter = File.AppendText((string)Paths.ConfigFile))
                    streamWriter.WriteLine("{}");
            }
            this.UserPath = pathTextBox.Text;
            ApplicationConfig applicationConfig = ConfigLoader.Instance.ApplicationConfig;
            applicationConfig.TestLabPath = this.UserPath;
            applicationConfig.isLoggerLocationVisible = this.isCheckedLocation;
            if (JsonWriter<ApplicationConfig>.WriteObject(applicationConfig, (string)Paths.ConfigFile))
            {
                Dialogs.PathModified(this.UserPath, SettingsViewModel.logger);
                BuildsLoader.TestLabPath = ConfigLoader.Instance.ApplicationConfig.TestLabPath;
            }
            else
                Dialogs.SavePathError(this.UserPath, SettingsViewModel.logger);
            System.Windows.Application.Current.Shutdown();
            System.Windows.Forms.Application.Restart();
        }
        #endregion
    }
}