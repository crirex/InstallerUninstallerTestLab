using Commmon.Utils.Readers;
using Common.Utils;
using Common.Utils.Constants;
using Installer.Utils;
using Logger.Utils;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using Uninstaller.Model;

namespace Uninstaller.ViewModel
{
    internal class UninstallerViewModel : NotifyPropertyChanged
    {
        #region Constructors
        public UninstallerViewModel()
        {
            LoadData();
        }
        #endregion

        #region Private fields
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private ObservableCollection<NameObject> list;
        private NameObject selectedUninstallItem;
        private bool uninstallBarInProgress;
        #endregion

        #region Properties
        public bool UninstallBarInProgress
        {
            get
            {
                return this.uninstallBarInProgress;
            }
            set
            {
                this.uninstallBarInProgress = value;
                this.RefreshCommand.RaiseCanExecuteChanged();
                this.OnPropertyChanged(nameof(UninstallBarInProgress));
            }
        }
        public ObservableCollection<NameObject> List
        {
            get
            {
                return this.list;
            }
            set
            {
                this.list = value;
                this.OnPropertyChanged(nameof(List));
            }
        }
        public NameObject SelectedUninstallItem
        {
            get
            {
                return this.selectedUninstallItem;
            }
            set
            {
                this.selectedUninstallItem = value;
                this.UninstallProgramCommand.RaiseCanExecuteChanged();
                this.RefreshCommand.RaiseCanExecuteChanged();
                this.OnPropertyChanged(nameof(SelectedUninstallItem));
            }
        }
        #endregion

        #region Commands
        private RelayCommand<DataGrid> uninstallProgramCommand;
        public RelayCommand<DataGrid> UninstallProgramCommand
        {
            get
            {
                if (this.uninstallProgramCommand == null)
                {
                    this.uninstallProgramCommand = new RelayCommand<DataGrid>(param => CanUninstallProgram(), async param => await UninstallProgramsAsync(param));
                }
                return this.uninstallProgramCommand;
            }
        }

        private RelayCommand refreshCommand;

        public RelayCommand RefreshCommand
        {
            get
            {
                if (this.refreshCommand == null)
                {
                    this.refreshCommand = new RelayCommand(param => CanRefreshProgram(), async param => await RefreshAsync());
                }
                return this.refreshCommand;
            }
        }
        #endregion

        #region Private methods
        private void LoadData()
        {

        }

        private async Task RefreshAsync()
        {
            try
            {
                LoggerUtils.LogMessage("Search installed components will start", (LogLevel)LogLevel.Info, UninstallerViewModel.logger);
                Stopwatch watch = Stopwatch.StartNew();
                this.UninstallBarInProgress = true;
                await Task.Run((Action)(() => this.List = new ObservableCollection<NameObject>((IEnumerable<NameObject>)this.GetProducts())));
                watch.Stop();
                DateTime elapsedTime = DateTime.MinValue.AddMilliseconds((double)watch.ElapsedMilliseconds);
                Dialogs.ElapsedTime("Search installed components", elapsedTime, UninstallerViewModel.logger);
                watch = (Stopwatch)null;
            }
            catch (Exception ex)
            {
                this.List = new ObservableCollection<NameObject>();
                Dialogs.UnknownError(ex.Message, UninstallerViewModel.logger);
            }
            finally
            {
                this.UninstallBarInProgress = false;
                this.SaveAllApplicationsFound();
            }
        }

        private void SaveAllApplicationsFound()
        {
            using (File.AppendText((string)Paths.AllApplications))
                ;
            System.Collections.Generic.List<string> stringList1 = new System.Collections.Generic.List<string>();
            System.Collections.Generic.List<string> stringList2 = new System.Collections.Generic.List<string>();
            string convertedName = "";
            string str1 = File.ReadAllText((string)Paths.AllApplications);
            char[] separator = new char[2] { '\n', '\r' };
            foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                stringList1.Add(str2);
            using (StreamWriter streamWriter = File.AppendText((string)Paths.AllApplications))
            {
                foreach (NameObject nameObject in (Collection<NameObject>)this.List)
                {
                    convertedName = this.ConvertApplicationName(nameObject.Name);
                    if (!stringList1.Exists((Predicate<string>)(token => convertedName.Equals(token))))
                        streamWriter.WriteLine(convertedName);
                }
            }
        }

        private string ConvertApplicationName(string name)
        {
            name = new Regex(" [0-9.]+", RegexOptions.IgnoreCase).Replace(name, "");
            name = name.Replace("Simcenter ", "");
            return name;
        }

        private bool CanUninstallProgram()
        {
            return SelectedUninstallItem != null && !UninstallBarInProgress;
        }

        private bool CanRefreshProgram()
        {
            return !UninstallBarInProgress;
        }

        private async Task UninstallProgramsAsync(DataGrid uninstallDataGrid)
        {
            if ((uint)uninstallDataGrid.SelectedItems.Count <= 0U)
                return;
            try
            {
                ObservableCollection<NameObject> selectedItemsList = new ObservableCollection<NameObject>();
                this.CopyListToList(uninstallDataGrid.SelectedItems, (IList)selectedItemsList);
                this.UninstallBarInProgress = true;
                if (this.IsDependencyFound(selectedItemsList))
                {
                    this.UninstallBarInProgress = false;
                }
                else
                {
                    LoggerUtils.LogMessage("Uninstall opperation will start", (LogLevel)LogLevel.Info, UninstallerViewModel.logger);
                    Stopwatch watch = Stopwatch.StartNew();
                    await this.UninstallListOfPrograms(selectedItemsList);
                    watch.Stop();
                    DateTime elapsedTime = DateTime.MinValue.AddMilliseconds((double)watch.ElapsedMilliseconds);
                    Dialogs.ElapsedTime("Uninstall opperation", elapsedTime, UninstallerViewModel.logger);
                    selectedItemsList = (ObservableCollection<NameObject>)null;
                    watch = (Stopwatch)null;
                }
            }
            catch (Exception ex)
            {
                Dialogs.UnknownError(ex.Message, UninstallerViewModel.logger);
            }
            finally
            {
                this.UninstallBarInProgress = false;
            }
        }

        private bool IsDependencyFound(ObservableCollection<NameObject> selectedItemsList)
        {
            Dictionary<string, System.Collections.Generic.List<string>> dictionary = JsonReader<Dictionary<string, System.Collections.Generic.List<string>>>.ReadObject((string)Paths.ApplicationDependencies);
            foreach (NameObject selectedItems in (Collection<NameObject>)selectedItemsList)
            {
                try
                {
                    foreach (string str in dictionary[this.ConvertApplicationName(selectedItems.Name)])
                    {
                        if (this.isDependencyInstalled(str) && !this.isDependencyUninstalling(selectedItemsList, str))
                        {
                            Dialogs.DependencyFoundError(this.ConvertApplicationName(selectedItems.Name), str, UninstallerViewModel.logger);
                            return true;
                        }
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        private bool isDependencyUninstalling(ObservableCollection<NameObject> selectedItemsList, string itemName)
        {
            foreach (NameObject selectedItems in (Collection<NameObject>)selectedItemsList)
            {
                if (this.ConvertApplicationName(selectedItems.Name) == itemName)
                    return true;
            }
            return false;
        }

        private bool isDependencyInstalled(string itemName)
        {
            foreach (NameObject nameObject in (Collection<NameObject>)this.List)
            {
                if (this.ConvertApplicationName(nameObject.Name) == itemName)
                    return true;
            }
            return false;
        }

        private async Task UninstallListOfPrograms(ObservableCollection<NameObject> toRemoveList)
        {
            foreach (NameObject toRemove in (Collection<NameObject>)toRemoveList)
            {
                NameObject product = toRemove;
                await Task.Run((Action)(() =>
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("WMIC PRODUCT WHERE \"NAME='{0}'\" call uninstall; shutdown /a", (object)product.Name);
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        powerShell.AddScript(stringBuilder.ToString());
                        powerShell.Invoke();
                    }
                }));
                this.List.Remove(product);
            }
        }

        private void CopyListToList(IList firstlist, IList secondlist)
        {
            foreach (NameObject nameObject in (IEnumerable)firstlist)
                secondlist.Add((object)nameObject);
        }

        private ObservableCollection<NameObject> GetProducts()
        {
            ObservableCollection<NameObject> observableCollection = new ObservableCollection<NameObject>();
            foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher(new ObjectQuery("SELECT * FROM Win32_Product WHERE Name LIKE '%Simcenter%' ")).Get())
                observableCollection.Add(new NameObject()
                {
                    Name = managementBaseObject.GetPropertyValue("Name").ToString()
                });
            return observableCollection;
        }
        #endregion
    }
}
