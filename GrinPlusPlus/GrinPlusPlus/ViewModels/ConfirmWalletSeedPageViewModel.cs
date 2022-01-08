using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class ConfirmWalletSeedPageViewModel : ViewModelBase
    {
        private bool _isValid = false;
        public bool IsValid
        {
            get => _isValid;
            set => SetProperty(ref _isValid, value);
        }

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

        private ObservableCollection<string> _displayedWalletSeedWordsList;
        public ObservableCollection<string> DisplayedWalletSeedWordsList
        {
            get
            {
                return _displayedWalletSeedWordsList;
            }
            set
            {
                SetProperty(ref _displayedWalletSeedWordsList, value);
            }
        }

        public DelegateCommand<object> UnfocusedEntryCommand { get; private set; }

        public ConfirmWalletSeedPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            UnfocusedEntryCommand = new DelegateCommand<object>(UnfocusedEntry);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            WalletSeed = await SecureStorage.GetAsync("wallet_seed");
            WalletSeedWordsList = new ObservableCollection<string>(WalletSeed.Split(' ').ToList());

            var walletSeed = new ObservableCollection<string>(WalletSeed.Split(' ').ToList());

            int hiddenWords = 0;
            do
            {
                foreach (string word in walletSeed)
                {
                    Random r = new Random();
                    int index = r.Next(0, 24);
                    if (!string.IsNullOrEmpty(walletSeed[index]))
                    {
                        walletSeed[index] = string.Empty;
                        hiddenWords++;
                        break;
                    }
                }
            } while (hiddenWords <= 4);

            DisplayedWalletSeedWordsList = walletSeed;
        }

        private void UnfocusedEntry(object param)
        {
            string word = (string)param;

            int index = WalletSeedWordsList.IndexOf(word);

            if (index == -1)
            {
                return;
            }

            DisplayedWalletSeedWordsList[index] = word;

            IsValid = string.Join(" ", DisplayedWalletSeedWordsList.ToArray()) == WalletSeed;
        }
    }
}
