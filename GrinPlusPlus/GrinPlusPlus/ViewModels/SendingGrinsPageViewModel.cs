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
using System.Threading;
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

        private string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
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

        private bool _isMessageLabelVisible = false;
        public bool IsMessageLabelVisible
        {
            get { return _isMessageLabelVisible; }
            set { SetProperty(ref _isMessageLabelVisible, value); }
        }

        public DelegateCommand SendUsingTorCommand => new DelegateCommand(SendUsingTor);

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

            if (await CrossFingerprint.Current.IsAvailableAsync(true))
            {
                _cancel = new CancellationTokenSource();

                var message = AppResources.ResourceManager.GetString("ConfirmIdentity");

                var dialogConfig = new AuthenticationRequestConfiguration("Fingerprint", message)
                {
                    CancelTitle = null,
                    FallbackTitle = null,
                    AllowAlternativeAuthentication = true
                };

                var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

                if (!result.Authenticated)
                {
                    return;
                }
            }

            await NavigationService.NavigateAsync("SendGrinsUsingTorPage",
                new NavigationParameters
                {
                    { "address", Address },
                    { "message", Message },
                    { "amount", Amount },
                    { "max", SendMax }
                }
            );
        }

        public SendingGrinsPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
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
                if(!string.IsNullOrEmpty(Address))
                {
                    IsAddressLabelVisible = true;
                }
            }

            if (parameters.ContainsKey("message"))
            {
                Message = (string)parameters["message"];
                if (!string.IsNullOrEmpty(Message))
                {
                    IsMessageLabelVisible = true;
                }
            }

            if (!SendMax)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        FeeEstimation estimation = await DataProvider.EstimateFee(await SecureStorage.GetAsync("token"), Amount);
                        Fee = (estimation.Fee / Math.Pow(10, 9)).ToString("F9");
                    }
                    catch (Exception ex)
                    {
                        await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                    }
                });
            }
        }

        public DelegateCommand CancelCommand => new DelegateCommand(Cancel);

        async void Cancel()
        {
            await NavigationService.GoBackToRootAsync();
        }
    }
}
