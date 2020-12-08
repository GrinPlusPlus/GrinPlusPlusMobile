using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;

using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class WalletLoginPageViewModel : ViewModelBase
    {
        private string _exceptionMessage = "";
        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set { SetProperty(ref _exceptionMessage, value.Trim()); }
        }

        private string _username = "";
        public string Username
        {
            get => _username.Trim();
            set => SetProperty(ref _username, value);
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isIdle = true;
        public bool IsIdle
        {
            get => _isIdle;
            set => SetProperty(ref _isIdle, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password.Trim();
            set => SetProperty(ref _password, value);
        }

        public DelegateCommand OpenWalletCommand => new DelegateCommand(OpenWallet);

        public DelegateCommand DeleteWalletCommand => new DelegateCommand(DeleteWalletC);

        public WalletLoginPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("username"))
            {
                Username = parameters.GetValue<string>("username");
            }
        }

        private async void OpenWallet()
        {
            try
            {
                var torAddress = await SecureStorage.GetAsync("tor_address");
                if (!string.IsNullOrEmpty(torAddress))
                {
                    try
                    {
                        await DataProvider.DeleteONION(torAddress);
                        SecureStorage.Remove("tor_address");
                    }
                    catch (Exception ex)
                    {
                        ExceptionMessage = ex.Message;
                    }
                }

                ExceptionMessage = string.Empty;

                IsBusy = true;
                IsIdle = false;

                if (string.IsNullOrEmpty(Password))
                {
                    throw new Exception(AppResources.ResourceManager.GetString("PasswordCanNotBeEmpty"));
                }

                var wallet = await DataProvider.DoLogin(Username, Password);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await SecureStorage.SetAsync("token", wallet.Token);
                    await SecureStorage.SetAsync("username", Username);
                    await SecureStorage.SetAsync("slatepack_address", wallet.SlatepackAdddress);
                    await SecureStorage.SetAsync("tor_address", wallet.TorAdddress ?? "");

                    await NavigationService.NavigateAsync("OpeningWalletPage");
                });
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
                IsIdle = true;
            }
        }

        private async void DeleteWalletC()
        {
            try
            {
                ExceptionMessage = string.Empty;

                IsBusy = true;
                IsIdle = false;

                if (string.IsNullOrEmpty(Password))
                {
                    throw new Exception(AppResources.ResourceManager.GetString("PasswordCanNotBeEmpty"));
                }
                var wallet = await DataProvider.DeleteWallet(Username, Password);

                await NavigationService.GoBackToRootAsync();
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
                IsIdle = true;
            }
        }

    }
}
