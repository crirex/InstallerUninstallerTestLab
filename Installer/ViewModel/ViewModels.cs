using Common.VML;
using Installer.ViewModel;
using InstallerUninstallerTestLab.ViewModel;
using Logger.ViewModel;
using Uninstaller.ViewModel;

namespace ViewModel
{
    internal class ViewModels
    {
        #region Properties
        public ViewModelLocator ViewModelLocator { get; private set; } = new ViewModelLocator();
        #endregion

        #region ViewModels
        public InstallerViewModel InstallerVM => ViewModelLocator.CreateViewModel<InstallerViewModel>();
        public UninstallerViewModel UninstallerVM => ViewModelLocator.CreateViewModel<UninstallerViewModel>();
        public LoggerViewModel LoggerVM => ViewModelLocator.CreateViewModel<LoggerViewModel>();
        public AppViewModel AppVM => ViewModelLocator.CreateViewModel<AppViewModel>();
        public SettingsViewModel SettingsVM => ViewModelLocator.CreateViewModel<SettingsViewModel>();
        public DependenciesViewModel DependenciesVM => ViewModelLocator.CreateViewModel<DependenciesViewModel>();
        #endregion
    }
}