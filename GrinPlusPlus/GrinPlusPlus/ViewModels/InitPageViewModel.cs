using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class InitPageViewModel : ViewModelBase
    {
        private string status = "";
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private double _progressBar = 0;
        public double ProgressBarr
        {
            get { return _progressBar; }
            set { SetProperty(ref _progressBar, value); }
        }

        private int _progressPercentage = 0;
        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set { SetProperty(ref _progressPercentage, value); }
        }

        public InitPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Status = AppResources.ResourceManager.GetString("InitializingNode");

            Preferences.Set("balance_spendable", 0);
            Preferences.Set("balance_locked", 0);
            Preferences.Set("balance_immature", 0);
            Preferences.Set("balance_unconfirmed", 0);
            Preferences.Set("balance_total", 0);

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var status = await DataProvider.GetNodeStatus();
                        Status = status.SyncStatus;

                        if (Status.Equals("Running"))
                        {
                            ProgressBarr = 1;
                            ProgressPercentage = 100;

                            Preferences.Clear();
                            SecureStorage.RemoveAll();

                            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
                        }
                        else
                        {
                            ProgressBarr = status.ProgressPercentage / 100;
                            ProgressPercentage = (int)Math.Round(ProgressBarr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                return !Status.Equals("Running");
            });
        }

    }
}
