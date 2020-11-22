using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;

namespace GrinPlusPlus.ViewModels
{
    public class SendingGrinsPageViewModel : ViewModelBase
    {
        private double _amount = 0;
        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private double _fee = 0;
        public double Fee
        {
            get { return _fee; }
            set { SetProperty(ref _fee, value); }
        }

        private string[] _inputs;
        public string[] Inputs
        {
            get { return _inputs; }
            set { SetProperty(ref _inputs, value); }
        }

        private string _address = "";
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DelegateCommand UsingTorButtonClickedCommand => new DelegateCommand(UsingTorButtonClicked);

        async void UsingTorButtonClicked()
        {
            await NavigationService.NavigateAsync("SendGrinsUsingTorPage",
                new NavigationParameters
                {
                    { "address", Address },
                    { "message", string.IsNullOrEmpty(Message) ? "" : Message },
                    { "amount", Amount },
                    { "inputs", Inputs }
                }
            );
        }

        public DelegateCommand UsingSlatepackButtonClickedCommand => new DelegateCommand(UsingSlatepackButtonClicked);

        async void UsingSlatepackButtonClicked()
        {
            await NavigationService.NavigateAsync("SendGrinsUsingQRPage",
                new NavigationParameters
                {
                    { "address", Address },
                    { "message", string.IsNullOrEmpty(Message) ? "" : Message },
                    { "amount", Amount },
                    { "inputs", Inputs }
                }
            );
        }

        public DelegateCommand UsingNFCButtonClickedCommand => new DelegateCommand(UsingNFCButtonClicked);

        async void UsingNFCButtonClicked()
        {
            await NavigationService.NavigateAsync("SendGrinsUsingNFCPage",
                new NavigationParameters
                {
                    { "address", Address },
                    { "message", string.IsNullOrEmpty(Message) ? "" : Message },
                    { "amount", Amount },
                    { "inputs", Inputs }
                }
            );
        }

        public SendingGrinsPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("amount"))
            {
                Amount = (double)parameters["amount"];
            }

            if (parameters.ContainsKey("fee"))
            {
                Fee = (double)parameters["fee"];
            }

            if (parameters.ContainsKey("address"))
            {
                Address = (string)parameters["address"];
            }

            if (parameters.ContainsKey("message"))
            {
                Message = (string)parameters["message"];
            }

            if (parameters.ContainsKey("inputs"))
            {
                Inputs = (string[])parameters["inputs"];
            }
        }

        public DelegateCommand CancelButtonClickedCommand => new DelegateCommand(CancelButtonClicked);

        async void CancelButtonClicked()
        {
            await NavigationService.GoBackToRootAsync();
        }
    }
}
