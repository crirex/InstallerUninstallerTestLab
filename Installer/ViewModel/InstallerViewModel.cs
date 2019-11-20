using Common.Enums;
using Common.Utils;
using Common.Utils.Constants;
using Installer.Actions;
using Installer.Exceptions;
using Installer.Model;
using Installer.Services;
using Installer.Utils;
using Installer.Utils.Extensions;
using InstallerUninstallerTestLab.View;
using Logger.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Version = Common.Enums.Version;

namespace Installer.ViewModel
{
    internal sealed class InstallerViewModel : NotifyPropertyChanged, IListener
    {
        #region Constructors
        public InstallerViewModel()
        {
            
            try
            {
                BuildsLoader = new BuildsLoader();
                BusyBuildChecker = new BusyBuildChecker(this);
                LoadData();
            }
            catch (Exception ex)
            {
                Dialogs.UnknownPathError(ex.Message, logger);
            }
        }
        #endregion

        #region Private fields
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private static bool installBarInProgress = false;
        private Architecture? selectedArchitecture;
        private Version? selectedVersion;
        private string selectedYear;
        private Build selectedBuild;
        private ObservableCollection<Build> builds;
        private readonly AppView mainView;
        #endregion

        #region Properties
        public IEnumerable<Architecture> Architectures
        {
            get => Enum.GetValues(typeof(Architecture)).Cast<Architecture>();
        }
        public Architecture? SelectedArchitecture
        {
            get => selectedArchitecture;
            set
            {
                this.selectedArchitecture = value;
                GetBuildsCommand.RaiseCanExecuteChanged();
            }
        }
        public Version? SelectedVersion
        {
            get => selectedVersion;
            set
            {
                this.selectedVersion = value;
                GetBuildsCommand.RaiseCanExecuteChanged();
            }
        }
        public string SelectedYear
        {
            get => selectedYear;
            set
            {
                this.selectedYear = value;
                GetBuildsCommand.RaiseCanExecuteChanged();
            }
        }
        public Build SelectedBuild
        {
            get => selectedBuild;
            set
            {
                this.selectedBuild = value;
                InstallBuildCommand.RaiseCanExecuteChanged();
            }
        }
        public ObservableCollection<Build> Builds
        {
            get => builds;
            set
            {
                this.builds = value;
                OnPropertyChanged(propertyName: nameof(Builds));
            }
        }
        public bool InstallBarInProgress
        {
            get
            {
                return InstallerViewModel.installBarInProgress;
            }
            set
            {
                InstallerViewModel.installBarInProgress = value;
                this.GetBuildsCommand.RaiseCanExecuteChanged();
                this.InstallBuildCommand.RaiseCanExecuteChanged();
                this.OnPropertyChanged(nameof(InstallBarInProgress));
            }
        }
        public BuildsLoader BuildsLoader { get; private set; }
        public BusyBuildChecker BusyBuildChecker { get; private set; }
        #endregion

        #region Commands
        private RelayCommand getBuildsCommand;
        public RelayCommand GetBuildsCommand
        {
            get
            {
                if (this.getBuildsCommand == null)
                {
                    this.getBuildsCommand = new RelayCommand(param => CanGetBuilds(), async param => await this.TryGetBuildsAsync());
                }
                return this.getBuildsCommand;
            }
        }
        private RelayCommand<DataGrid> installBuildCommand;
        public RelayCommand<DataGrid> InstallBuildCommand
        {
            get
            {
                if (this.installBuildCommand == null)
                {
                    this.installBuildCommand = new RelayCommand<DataGrid>(param => CanInstallBuild(), async param => await InstallBuildAsync(param));
                }
                return this.installBuildCommand;
            }
        }
        #endregion

        #region Private methods
        private void LoadData()
        {
            try
            {
                if (BuildsLoader.LoadBuilds())
                {
                    this.mainView.Visibility = Visibility.Visible;
                }
            }
            catch (PathAccessDeniedException ex)
            {
                Dialogs.UnknownPathError(ex.Message, InstallerViewModel.logger);
            }
            catch (PathNotFoundException ex)
            {
                Dialogs.UnknownPathError(ex.Message, InstallerViewModel.logger);
            }
            catch (UnknownException ex)
            {
                Dialogs.UnknownPathError(ex.Message, InstallerViewModel.logger);
            }
        }

        private bool CanGetBuilds()
        {
            return SelectedVersion.HasValue && SelectedArchitecture.HasValue && (SelectedYear != null);
        }
        private bool CanInstallBuild()
        {
            return (SelectedBuild != null ? !SelectedBuild.Busy : false) && !InstallBarInProgress;
        }

        private class SortByDateByDescendingOrder : IComparer<Build>
        {
            public int Compare(Build x, Build y)
            {
                if (x.Date < y.Date)
                    return 1;
                return x.Date > y.Date ? -1 : 0;
            }
        }

        private async Task TryGetBuildsAsync()
        {
            try
            {
                this.InstallBarInProgress = true;
                Stopwatch watch = Stopwatch.StartNew();
                string year = this.SelectedYear.Substring(this.SelectedYear.Length - 2);
                Architecture? selectedArchitecture = this.SelectedArchitecture;
                Architecture architecture1 = (Architecture)1;
                string architecture = (string)(selectedArchitecture.GetValueOrDefault() == architecture1 & selectedArchitecture.HasValue ? Files.X64Architecture : Files.X32Architecture);
                LoggerUtils.LogMessage("Search for builds will start", (LogLevel)LogLevel.Info, InstallerViewModel.logger);
                List<Build> results = new List<Build>();
                await Task.Run((Action)(() =>
                {
                    ((IEnumerable<string>)Directory.GetDirectories(BuildsLoader.TestLabPath)).Where<string>((Func<string, bool>)(x => Regex.IsMatch(x, year))).Where<string>((Func<string, bool>)(x => Regex.IsMatch(x, this.SelectedVersion.ToString()))).ToList<string>().ForEach((Action<string>)(directory => results.AddRange((IEnumerable<Build>)((IEnumerable<string>)Directory.GetFiles(directory, (string)Files.Executable, SearchOption.AllDirectories)).Where<string>((Func<string, bool>)(path => Directory.GetParent(path).Parent.Parent.Name == architecture)).Select<string, Build>((Func<string, Build>)(path => new Build()
                    {
                        Year = this.SelectedYear
                    }.FromPath(path))).ToList<Build>())));
                    results.Sort((IComparer<Build>)new InstallerViewModel.SortByDateByDescendingOrder());
                    this.Builds = new ObservableCollection<Build>(results);
                    this.BusyBuildChecker.BusyBuilds = this.GetBusyBuilds(results);
                }));
                watch.Stop();
                DateTime elapsedTime = DateTime.MinValue.AddMilliseconds((double)watch.ElapsedMilliseconds);
                Dialogs.ElapsedTime("Search for builds", elapsedTime, InstallerViewModel.logger);
                watch = (Stopwatch)null;
            }
            catch
            {
                this.Builds = new ObservableCollection<Build>();
            }
            finally
            {
                this.InstallBarInProgress = false;
            }
        }

        private async Task InstallBuildAsync(DataGrid buildsList)
        {
            string path = (buildsList.SelectedItem as Build)?.Path;
            buildsList.SelectedItem = (object)null;
            LoggerUtils.LogMessage("Install opperation will start", (LogLevel)LogLevel.Info, InstallerViewModel.logger);
            try
            {
                this.InstallBarInProgress = true;
                Stopwatch watch = Stopwatch.StartNew();
                await Task.Run((Action)(() => this.Install(path)));
                watch.Stop();
                DateTime elapsedTime = DateTime.MinValue.AddMilliseconds((double)watch.ElapsedMilliseconds);
                Dialogs.ElapsedTime("Install opperation", elapsedTime, InstallerViewModel.logger);
                watch = (Stopwatch)null;
            }
            catch
            {
                Dialogs.InstallationFailed(path, InstallerViewModel.logger);
            }
            finally
            {
                this.InstallBarInProgress = false;
            }
        }

        private List<Build> GetBusyBuilds(List<Build> builds)
        {
            List<Build> busyBuilds = new List<Build>();
            builds.ForEach(build =>
            {
                if (build.Busy)
                {
                    busyBuilds.Add(build);
                }
            });
            return busyBuilds;
        }
        private void Install(string path)
        {
            Thread.Sleep(2000);
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = "/quiet";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Exited += (EventHandler)((sender, e) => this.InstallBarInProgress = false);
            process.Start();
            process.WaitForExit();
        }


        #endregion

        #region IListener
        public void Update()
        {
            this.LoadData();
            Task.Run((Func<Task>)(async () => await this.TryGetBuildsAsync()));
        }
        #endregion
    }
}