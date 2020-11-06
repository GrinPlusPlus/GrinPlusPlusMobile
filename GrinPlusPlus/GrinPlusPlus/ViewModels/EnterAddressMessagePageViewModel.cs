using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using ZXing.Mobile;
using static ZXing.Mobile.MobileBarcodeScanningOptions;

namespace GrinPlusPlus.ViewModels
{
    public class EnterAddressMessagePageViewModel : ViewModelBase
    {
        private string _address;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private ZXing.Result _result;
        public ZXing.Result Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        private bool _isAnalyzing;
        public bool IsAnalyzing
        {
            get { return _isAnalyzing; }
            set { SetProperty(ref _isAnalyzing, value); }
        }

        private bool _isScanning;
        public bool IsScanning
        {
            get { return _isScanning; }
            set { SetProperty(ref _isScanning, value); }
        }

        private MobileBarcodeScanningOptions _scannerOptions;
        public MobileBarcodeScanningOptions ScannerOptions
        {
            get { return _scannerOptions; }
            set { SetProperty(ref _scannerOptions, value); }
        }

        public DelegateCommand SendButtonClickedCommand => new DelegateCommand(SendButtonClicked);

        async void SendButtonClicked()
        {
            await NavigationService.NavigateAsync("SendingGrinsPage", 
                new NavigationParameters
                {
                    { "addres", Address },
                    { "message", Message }
                }
            );
        }

        public DelegateCommand<ZXing.Result> OnScanResultCommand => new DelegateCommand<ZXing.Result>(OnScanResult);

        private void OnScanResult(ZXing.Result result)
        {
            Address = result.Text;
        }

        public DelegateCommand OnQRButtonClickedCommand => new DelegateCommand(OnQRButtonClicked);

        private void OnQRButtonClicked()
        {
            IsScanning = !IsScanning;
        }

        public EnterAddressMessagePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Address = "";
            IsScanning = false;
            ScannerOptions = new MobileBarcodeScanningOptions() {
                PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.QR_CODE },
                CameraResolutionSelector = new CameraResolutionSelectorDelegate(SelectLowestResolutionMatchingDisplayAspectRatio),
            };
        }

        public CameraResolution SelectLowestResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
        {
            CameraResolution result = null;

            //a tolerance of 0.1 should not be recognizable for users
            double aspectTolerance = 0.1;
            
            //calculating our targetRatio
            var targetRatio = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Width;
            var targetHeight = DeviceDisplay.MainDisplayInfo.Height;
            var minDiff = double.MaxValue;
            
            //camera API lists all available resolutions from highest to lowest, perfect for us
            //making use of this sorting, following code runs some comparisons to select the lowest resolution that matches the screen aspect ratio
            //selecting the lowest makes QR detection actual faster most of the time
            foreach (var r in availableResolutions)
            {
                //if current ratio is bigger than our tolerance, move on
                //camera resolution is provided landscape ...
                if (Math.Abs(((double) r.Width / r.Height) - targetRatio) > aspectTolerance)
                    continue;
                else
                    if (Math.Abs(r.Height - targetHeight) < minDiff)
                    minDiff = Math.Abs(r.Height - targetHeight);
                    result = r;                
            }
            return result;
        }

    }
}
