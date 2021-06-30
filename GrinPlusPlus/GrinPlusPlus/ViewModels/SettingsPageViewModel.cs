using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using Xamarin.Essentials;


namespace GrinPlusPlus.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private int _confirmations = Settings.Confirmations;
        public int Confirmations
        {
            get => _confirmations;
            set => SetProperty(ref _confirmations, value);
        }

        private int _minimumPeers = Settings.MinimumPeers;
        public int MinimumPeers
        {
            get => _minimumPeers;
            set => SetProperty(ref _minimumPeers, value);
        }

        private int _maximumPeers = Settings.MaximumPeers;
        public int MaximumPeers
        {
            get => _maximumPeers;
            set => SetProperty(ref _maximumPeers, value);
        }

        private string _grinchckurl = Settings.GrinChckAPIURL;
        public string GrinChckAPIUrl
        {
            get => _grinchckurl.Trim();
            set => SetProperty(ref _grinchckurl, value);
        }

        public DelegateCommand OpenBackupWalletPageCommand => new DelegateCommand(OpenBackupWalletPage);

        public DelegateCommand UpdateNodeSettingsCommand => new DelegateCommand(UpdateNodeSetting);

        public DelegateCommand LogoutCommand => new DelegateCommand(Logout);

        public SettingsPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }

        private async void OpenBackupWalletPage()
        {
            await NavigationService.NavigateAsync("BackupWalletPage", new NavigationParameters { { "username", await SecureStorage.GetAsync("username") } });
        }

        async void Logout()
        {
            try
            {
                var token = await SecureStorage.GetAsync("token");
                await DataProvider.DoLogout(token);

                Preferences.Clear();
                SecureStorage.RemoveAll();

                Settings.IsLoggedIn = false;

                await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        async void UpdateNodeSetting()
        {
            try
            {
                await DataProvider.UpdateNodeSettings(MinimumPeers, MaximumPeers, Confirmations);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
