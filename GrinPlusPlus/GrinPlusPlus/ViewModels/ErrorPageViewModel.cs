using Prism.Commands;
using System;
using Xamarin.Essentials;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using Prism.Navigation;
using GrinPlusPlus.Api;
using Prism.Services.Dialogs;
using Prism.Services;

namespace GrinPlusPlus.ViewModels
{
    public class ErrorPageViewModel : ViewModelBase
    {
        public DelegateCommand SupportButtonClickedCommand => new DelegateCommand(SupportButtonClicked);
        public DelegateCommand ExportLogsButtonClickedCommand => new DelegateCommand(ExportLogsButtonClicked);
        public DelegateCommand DeleteChainButtonClickedCommand => new DelegateCommand(DeleteChainButtonClicked);

        async void SupportButtonClicked()
        {
            await Launcher.TryOpenAsync(new Uri("https://t.me/GrinPP"));
        }

        async void ExportLogsButtonClicked()
        {
            string zipPath = Path.Combine(Settings.BackendFolder, "logs.zip");
            try
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                ZipFile.CreateFromDirectory(Settings.LogsFolder, zipPath);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Logs",
                    File = new ShareFile(zipPath)
                });
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        async void DeleteChainButtonClicked()
        {
            var yes = await PageDialogService.DisplayAlertAsync("Confirmation", "Are you sure you want to delete the Blockchain Data?", "Yes", "No");
            if(yes)
            {
                Directory.Delete(Settings.DataFolder, true);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        public ErrorPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }
    }
}
