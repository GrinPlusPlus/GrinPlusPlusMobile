using Prism.Commands;
using Prism.Navigation;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Prism.Services.Dialogs;

namespace GrinPlusPlus.ViewModels
{
    public class ReceivePageViewModel : ViewModelBase
    {
        private string _address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x";
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        public DelegateCommand ShareAddressCommand => new DelegateCommand(ShareAddress);

        private async void ShareAddress()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = Address,
                Title = "Wallet Address"
            });
        }

        public ReceivePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
