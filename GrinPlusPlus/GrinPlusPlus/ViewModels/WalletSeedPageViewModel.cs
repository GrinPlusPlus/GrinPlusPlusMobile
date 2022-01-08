using GrinPlusPlus.Api;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class WalletSeedPageViewModel : ViewModelBase
    {
        private string _walletSeed = "";
        public string WalletSeed
        {
            get => _walletSeed.Trim();
            set => SetProperty(ref _walletSeed, value);
        }

        private ObservableCollection<string> _walletSeedWordsList;
        public ObservableCollection<string> WalletSeedWordsList
        {
            get
            {
                return _walletSeedWordsList;
            }
            set
            {
                SetProperty(ref _walletSeedWordsList, value);
            }
        }


        public WalletSeedPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            WalletSeed = await SecureStorage.GetAsync("wallet_seed");
            WalletSeedWordsList = new ObservableCollection<string>(WalletSeed.Split(' ').ToList());
        }
    }
}
