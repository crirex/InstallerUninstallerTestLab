using Commmon.Utils.Readers;
using Commmon.Utils.Writers;
using Common.Utils;
using Common.Utils.Constants;
using Installer.Utils;
using InstallerUninstallerTestLab.Model;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Uninstaller.Model;

namespace InstallerUninstallerTestLab.ViewModel
{
    internal class DependenciesViewModel : NotifyPropertyChanged
    {
        #region Private fields
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private ObservableCollection<NameObject> leftList;
        private ObservableCollection<NameObject> centerList;
        private ObservableCollection<ListDependencyObject> rightList;
        private NameObject selectedLeftItem;
        private NameObject selectedCenterItem;
        private ListDependencyObject selectedRightItem;
        #endregion

        #region Constructors
        public DependenciesViewModel()
        {
            this.LoadData();
            this.LoadRightList();
        }
        #endregion

        #region Commands
        private RelayCommand createDependencyCommand;
        public RelayCommand CreateDependencyCommand
        {
            get
            {
                if (this.createDependencyCommand == null)
                    this.createDependencyCommand = new RelayCommand((Predicate<object>)(param => this.CanCreateDependency()), (Action<object>)(param => this.CreateDependency()));
                return this.createDependencyCommand;
            }
        }

        private RelayCommand<DataGrid> deleteCommand;
        public RelayCommand<DataGrid> DeleteCommand
        {
            get
            {
                if (this.deleteCommand == null)
                    this.deleteCommand = new RelayCommand<DataGrid>((Predicate<DataGrid>)(param => this.CanDeleteDependency()), (Action<DataGrid>)(param => this.DeleteDependency(param)));
                return this.deleteCommand;
            }
        }

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

        private RelayCommand<Window> okCommand;
        public RelayCommand<Window> OkCommand
        {
            get
            {
                if (this.okCommand == null)
                    this.okCommand = new RelayCommand<Window>((Action<Window>)(param => this.Ok(param)));
                return this.okCommand;
            }
        }

        private RelayCommand refreshLists;
        public RelayCommand RefreshLists
        {
            get
            {
                if (this.refreshLists == null)
                    this.refreshLists = new RelayCommand((Action<object>)(param => this.LoadData()));
                return this.refreshLists;
            }
        }
        #endregion

        #region Properties
        public ObservableCollection<NameObject> LeftList
        {
            get
            {
                return this.leftList;
            }
            set
            {
                this.leftList = value;
                this.OnPropertyChanged(nameof(LeftList));
            }
        }

        public ObservableCollection<NameObject> CenterList
        {
            get
            {
                return this.centerList;
            }
            set
            {
                this.centerList = value;
                this.OnPropertyChanged(nameof(CenterList));
            }
        }

        public ObservableCollection<ListDependencyObject> RightList
        {
            get
            {
                return this.rightList;
            }
            set
            {
                this.rightList = value;
                this.OnPropertyChanged(nameof(RightList));
            }
        }

        public NameObject SelectedLeftItem
        {
            get
            {
                return this.selectedLeftItem;
            }
            set
            {
                this.selectedLeftItem = value;
                this.CreateDependencyCommand.RaiseCanExecuteChanged();
                this.LoadCenterList();
                this.OnPropertyChanged(nameof(SelectedLeftItem));
            }
        }

        public NameObject SelectedCenterItem
        {
            get
            {
                return this.selectedCenterItem;
            }
            set
            {
                this.selectedCenterItem = value;
                this.CreateDependencyCommand.RaiseCanExecuteChanged();
                this.OnPropertyChanged(nameof(SelectedCenterItem));
            }
        }

        public ListDependencyObject SelectedRightItem
        {
            get
            {
                return this.selectedRightItem;
            }
            set
            {
                this.selectedRightItem = value;
                this.DeleteCommand.RaiseCanExecuteChanged();
                this.OnPropertyChanged(nameof(SelectedRightItem));
            }
        }
        #endregion

        #region Private methods
        private void LoadData()
        {
            this.LeftList = new ObservableCollection<NameObject>();
            this.CenterList = new ObservableCollection<NameObject>();
            this.SelectedCenterItem = (NameObject)null;
            this.SelectedLeftItem = (NameObject)null;
            this.SelectedRightItem = (ListDependencyObject)null;
            this.LoadLeftList();
        }

        private void LoadLeftList()
        {
            using (File.AppendText((string)Paths.AllApplications))
                ;
            string str1 = File.ReadAllText((string)Paths.AllApplications);
            char[] separator = new char[2] { '\n', '\r' };
            foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                this.LeftList.Add(new NameObject() { Name = str2 });
            if (this.LeftList.Count != 0)
                return;
            Dialogs.NoApplications(DependenciesViewModel.logger);
        }

        private void LoadCenterList()
        {
            this.CenterList = new ObservableCollection<NameObject>();
            this.CopyListToList((IList)this.LeftList, (IList)this.CenterList);
            try
            {
                foreach (NameObject center in (Collection<NameObject>)this.CenterList)
                {
                    if (center.Name.Equals(this.SelectedLeftItem.Name))
                    {
                        this.CenterList.Remove(center);
                        break;
                    }
                }
                foreach (ListDependencyObject right in (Collection<ListDependencyObject>)this.rightList)
                {
                    if (right.FirstName.Equals(this.SelectedLeftItem.Name))
                    {
                        foreach (NameObject center in (Collection<NameObject>)this.CenterList)
                        {
                            if (center.Name.Equals(right.LastName))
                            {
                                this.CenterList.Remove(center);
                                break;
                            }
                        }
                    }
                    if (right.LastName.Equals(this.SelectedLeftItem.Name))
                    {
                        foreach (NameObject center in (Collection<NameObject>)this.CenterList)
                        {
                            if (center.Name.Equals(right.FirstName))
                            {
                                this.CenterList.Remove(center);
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                this.CenterList = new ObservableCollection<NameObject>();
            }
        }

        private void LoadRightList()
        {
            Dictionary<string, List<string>> dictionary = JsonReader<Dictionary<string, List<string>>>.ReadObject((string)Paths.ApplicationDependencies);
            try
            {
                foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary)
                {
                    foreach (string str in keyValuePair.Value)
                        this.RightList.Add(new ListDependencyObject()
                        {
                            FirstName = keyValuePair.Key,
                            LastName = str
                        });
                }
            }
            catch
            {
            }
        }

        private bool CanCreateDependency()
        {
            return this.SelectedLeftItem != null && this.SelectedCenterItem != null;
        }

        private void CreateDependency()
        {
            this.RightList.Add(new ListDependencyObject()
            {
                FirstName = this.SelectedLeftItem.Name,
                LastName = this.SelectedCenterItem.Name
            });
            this.LoadCenterList();
        }

        private bool CanDeleteDependency()
        {
            return this.SelectedRightItem != null;
        }

        private void DeleteDependency(DataGrid rightDataGrid)
        {
            ObservableCollection<ListDependencyObject> observableCollection = new ObservableCollection<ListDependencyObject>();
            this.CopyListToList(rightDataGrid.SelectedItems, (IList)observableCollection);
            foreach (ListDependencyObject dependencyObject in (Collection<ListDependencyObject>)observableCollection)
                this.RightList.Remove(dependencyObject);
            this.LoadCenterList();
        }

        private void Cancel(Window window)
        {
            this.RightList = new ObservableCollection<ListDependencyObject>();
            this.LoadRightList();
            window.Close();
        }

        private void Ok(Window window)
        {
            this.SaveDependenciesToFile();
            window.Close();
        }

        private void SaveDependenciesToFile()
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            foreach (ListDependencyObject right in (Collection<ListDependencyObject>)this.RightList)
            {
                try
                {
                    dictionary[right.FirstName].Add(right.LastName);
                }
                catch
                {
                    dictionary.Add(right.FirstName, new List<string>());
                    dictionary[right.FirstName].Add(right.LastName);
                }
            }
            if (JsonWriter<Dictionary<string, List<string>>>.WriteObject(dictionary, (string)Paths.ApplicationDependencies))
                Dialogs.DependenciesAdded(DependenciesViewModel.logger);
            else
                Dialogs.DependenciesAddedError(DependenciesViewModel.logger);
        }

        private void CopyListToList(IList firstlist, IList secondlist)
        {
            foreach (object obj in (IEnumerable)firstlist)
                secondlist.Add(obj);
        }
        #endregion
    }
}
