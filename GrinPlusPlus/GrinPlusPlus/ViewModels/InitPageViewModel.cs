using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class InitPageViewModel : ViewModelBase
    {
        private string status = Settings.Node.Status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private double _progressBar = Settings.Node.ProgressPercentage;
        public double ProgressBarr
        {
            get { return _progressBar; }
            set { SetProperty(ref _progressBar, value); }
        }

        private string _progressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");
        public string ProgressPercentage
        {
            get { return _progressPercentage; }
            set { SetProperty(ref _progressPercentage, value); }
        }

        private bool displayError = false;
        public bool DisplayError
        {
            get { return displayError; }
            set { SetProperty(ref displayError, value); }
        }

        private bool displayProgress = false;
        public bool DisplayProgress
        {
            get { return displayProgress; }
            set { SetProperty(ref displayProgress, value); }
        }

        private bool displayInitialMessage = true;
        public bool DisplayInitialMessage
        {
            get { return displayInitialMessage; }
            set { SetProperty(ref displayInitialMessage, value); }
        }

        private bool displayActivityIndicator = true;
        public bool DisplayActivityIndicator
        {
            get { return displayActivityIndicator; }
            set { SetProperty(ref displayActivityIndicator, value); }
        }

        private bool displaySuccessMessage = false;
        public bool DisplaySuccessMessage
        {
            get { return displaySuccessMessage; }
            set { SetProperty(ref displaySuccessMessage, value); }
        }
        

        public DelegateCommand SupportButtonClickedCommand => new DelegateCommand(SupportButtonClicked);

        async void SupportButtonClicked()
        {
            await Launcher.TryOpenAsync(new Uri("https://t.me/GrinPP"));
        }

        public InitPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Preferences.Clear();
            SecureStorage.RemoveAll();

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                if(DisplayError)
                {
                    return false;
                }

                Status = Settings.Node.Status;
                ProgressBarr = Settings.Node.ProgressPercentage;
                ProgressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");

                if (Settings.Node.Status.Equals("Running"))
                {
                    // Show Success Message
                    DisplayProgress = false;
                    DisplaySuccessMessage = true;
                    DisplayActivityIndicator = true;
                    DisplayInitialMessage = false;
                    DisplayError = false;

                    return false;
                } else
                {
                    DisplayInitialMessage = false;
                    DisplayProgress = true;
                }

                if (DisplaySuccessMessage)
                {
                    return false;
                }

                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                if (Settings.Node.Status.Equals("Disconnected"))
                {
                    // Show Error Message
                    DisplayProgress = false;
                    DisplaySuccessMessage = false;
                    DisplayActivityIndicator = false;
                    DisplayInitialMessage = false;
                    DisplayError = true;
                }
                return false;
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if(DisplaySuccessMessage)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
                    });

                    return false;
                }

                return true;
            });
        }
    }
}
