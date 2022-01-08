using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class SendGrinsUsingTorPageViewModel : ViewModelBase
    {
        private double _amount = -1;
        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string _address = string.Empty;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private SendingResponse _sendingResponse;
        public SendingResponse SendingResponse
        {
            get { return _sendingResponse; }
            set { SetProperty(ref _sendingResponse, value); }
        }

        private bool _isBusy;
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
            IsBusy = true;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("amount"))
            {
                Amount = (double)parameters["amount"];
            }

            if (parameters.ContainsKey("address"))
            {
                Address = (string)parameters["address"];
            }

            if (parameters.ContainsKey("max"))
            {
                SendMax = (bool)parameters["max"];
            }

            try
            {
                var token = await SecureStorage.GetAsync("token");
                SendingResponse = await DataProvider.SendGrins(token, Address, Amount, string.Empty, null, "SMALLEST", SendMax);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.GoBackToRootAsync();
                });
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;

                Debug.WriteLine(ex.Message);
            }

            IsBusy = false;
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
