using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;


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
                await DataProvider.DoLogout(await SecureStorage.GetAsync("token"));
                var torAddress = await SecureStorage.GetAsync("tor_address");
                if (!string.IsNullOrEmpty(torAddress))
                {
                    await DataProvider.DeleteONION(torAddress);
                    SecureStorage.Remove("tor_address");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Settings.IsLoggedIn = false;

            Preferences.Clear();
            SecureStorage.RemoveAll();

            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
        }
    }
}
