using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;

namespace GrinPlusPlus.ViewModels
{
    public class ReceiveTransactionDialogViewModel : ViewModelBase, IDialogAware
    {
        private string _slatepack;
        public string Slatepack
        {
            get => _slatepack;
            set => SetProperty(ref _slatepack, value);
        }

        public DelegateCommand CloseCommand { get; }

        public ReceiveTransactionDialogViewModel(INavigationService navigationService) : base(navigationService)
        {
            CloseCommand = new DelegateCommand(async() => {
                RequestClose(null); 
            });
        }

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
           
        }
    }
}
