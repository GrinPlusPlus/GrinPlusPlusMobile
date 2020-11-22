using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using ZXing;

namespace GrinPlusPlus.ViewModels
{
    public class QRScannerPageViewModel : ViewModelBase
    {
        private Result _result;
        public Result Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        private bool _isAnalyzing = true;
        public bool IsAnalyzing
        {
            get { return _isAnalyzing; }
            set { SetProperty(ref _isAnalyzing, value); }
        }

        private bool _isScanning = true;
        public bool IsScanning
        {
            get { return _isScanning; }
            set { SetProperty(ref _isScanning, value); }
        }

        public DelegateCommand<Result> OnScanResultCommand => new DelegateCommand<Result>(OnScanResult);

        private void OnScanResult(Result result)
        {
            NavigationService.GoBackAsync(new NavigationParameters { { "qr_scanner_result", result.Text } });
        }


        public QRScannerPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }
    }
}
