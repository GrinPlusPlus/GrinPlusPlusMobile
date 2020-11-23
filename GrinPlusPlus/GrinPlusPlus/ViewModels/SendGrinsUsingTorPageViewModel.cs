using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Linq;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class SendGrinsUsingTorPageViewModel : ViewModelBase
    {
        private double _amount = 0;
        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string _address = "";
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _message = "-";
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private SendingResponse _sendingResponse;
        public SendingResponse SendingResponse
        {
            get { return _sendingResponse; }
            set { SetProperty(ref _sendingResponse, value); }
        }


        private bool _isBusy = true;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private string _exceptionMessage;
        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set { SetProperty(ref _exceptionMessage, value); }
        }

        private bool _sendMax = false;
        public bool SendMax
        {
            get { return _sendMax; }
            set { SetProperty(ref _sendMax, value); }
        }

        public SendGrinsUsingTorPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("amount"))
            {
                Amount = (double)parameters["amount"];
            }

            if (parameters.ContainsKey("address"))
            {
                Address = (string)parameters["address"];
            }

            if (parameters.ContainsKey("message"))
            {
                Message = (string)parameters["message"];
            }

            if (parameters.ContainsKey("max"))
            {
                SendMax = (bool)parameters["max"];
            }

            if (!string.IsNullOrEmpty(Address) && Amount > 0)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        string token = await SecureStorage.GetAsync("token");
                        SendingResponse = await DataProvider.SendGrins(token, Address, Amount, Message, null, "SMALLEST", SendMax);
                        if(SendingResponse.Status.ToLower().Equals("finalized"))
                        {
                            await NavigationService.GoBackToRootAsync();
                        } else
                        {
                            await NavigationService.NavigateAsync("SendGrinsUsingQRPage", new NavigationParameters { { "sending_response", SendingResponse } });
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionMessage = ex.Message;
                    }
                    IsBusy = false;
                });
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            var formsNav = ((Prism.Common.IPageAware)NavigationService).Page.Navigation;
            var pageType = PageNavigationRegistry.GetPageType("SendGrinsUsingTorPage");
            var page = formsNav.NavigationStack.LastOrDefault(p => p.GetType() == pageType) ?? formsNav.ModalStack.LastOrDefault(p => p.GetType() == pageType);
            formsNav.RemovePage(page);
        }
    }
}
