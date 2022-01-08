using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class SendingGrinsPageViewModel : ViewModelBase
    {
        private CancellationTokenSource _cancel;

        private double _amount = 0;
        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string _fee = string.Empty;
        public string Fee
        {
            get { return _fee; }
            set { SetProperty(ref _fee, value); }
        }

        private string _address = string.Empty;
        public string Address
        {
            get { return _address.Trim(); }
            set { SetProperty(ref _address, value); }
        }

        private bool _sendMax = false;
        public bool SendMax
        {
            get { return _sendMax; }
            set { SetProperty(ref _sendMax, value); }
        }

        private bool _isAddressLabelVisible = false;
        public bool IsAddressLabelVisible
        {
            get { return _isAddressLabelVisible; }
            set { SetProperty(ref _isAddressLabelVisible, value); }
        }

        public DelegateCommand SendUsingTorCommand => new DelegateCommand(SendUsingTor);

        public SendingGrinsPageViewModel(INavigationService navigationService, IDataProvider dataProvider,
                                        IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("max"))
            {
                SendMax = (bool)parameters["max"];
            }

            if (parameters.ContainsKey("amount"))
            {
                Amount = (double)parameters["amount"];
            }

            if (parameters.ContainsKey("address"))
            {
                Address = (string)parameters["address"];
                if (!string.IsNullOrEmpty(Address))
                {
                    IsAddressLabelVisible = true;
                }
            }

            if (!SendMax)
            {
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        FeeEstimation estimation = await DataProvider.EstimateFee(await SecureStorage.GetAsync("token"), Amount);
                        Fee = (estimation.Fee / Math.Pow(10, 9)).ToString("F9");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);

                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                        });
                    }
                });
            }
        }

        public DelegateCommand CancelCommand => new DelegateCommand(Cancel);

        async void Cancel()
        {
            await NavigationService.GoBackToRootAsync();
        }

        async void SendUsingTor()
        {
            if (Amount <= 0)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var message = AppResources.ResourceManager.GetString("AmountCantBeNull");
                    await PageDialogService.DisplayAlertAsync("Error", message, "OK");
                });
                return;
            }

            FingerprintAvailability availability = await CrossFingerprint.Current.GetAvailabilityAsync();

            if (availability.Equals(FingerprintAvailability.Available) && await CrossFingerprint.Current.IsAvailableAsync())
            {
                _cancel = new CancellationTokenSource();

                var message = AppResources.ResourceManager.GetString("ConfirmIdentity");

                var dialogConfig = new AuthenticationRequestConfiguration("Fingerprint", message)
                {
                    CancelTitle = AppResources.ResourceManager.GetString("Cancel"),
                    FallbackTitle = null,
                    AllowAlternativeAuthentication = true
                };

                var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

                if (!result.Authenticated)
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(Address))
            {
                var token = await SecureStorage.GetAsync("token");
                await DataProvider.SendGrins(token, Address, Amount, string.Empty, null, "SMALLEST", SendMax);
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.GoBackToRootAsync();
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.NavigateAsync("SendGrinsUsingTorPage",
                            new NavigationParameters
                            {
                            { "address", Address },
                            { "amount", Amount },
                            { "max", SendMax }
                            }
                        );
                });
            }
            
        }
    }
}
