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

        private string _debugInformation = String.Empty;
        public string DebugInformation
        {
            get { return _debugInformation; }
            set { SetProperty(ref _debugInformation, value); }
        }

        public ErrorPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            var tor = Path.Combine(Settings.NativeLibraryDir, "libtor.so");
            var node = Path.Combine(Settings.NativeLibraryDir, "libgrin.so");

            var backendFolderFound = Directory.Exists(Settings.BackendFolder) ? "FOUND" : "NOT FOUND";
            var dataFolderFound = Directory.Exists(Settings.DataFolder) ? "FOUND" : "NOT FOUND";
            var logsFolderFound = Directory.Exists(Settings.LogsFolder) ? "FOUND" : "NOT FOUND";
            var nativeLibraryDiround = Directory.Exists(Settings.NativeLibraryDir) ? "FOUND" : "NOT FOUND";
            var torBinaryFound = File.Exists(tor) ? "FOUND" : "NOT FOUND";
            var nodeBinaryFound = File.Exists(node) ? "FOUND" : "NOT FOUND";

            DebugInformation += $"{Settings.BackendFolder} : {backendFolderFound}";
            DebugInformation += $"\n{Settings.DataFolder} : {dataFolderFound}";
            DebugInformation += $"\n{Settings.LogsFolder} : {logsFolderFound}";
            DebugInformation += $"\n{Settings.NativeLibraryDir} : {nativeLibraryDiround}";
            DebugInformation += $"\n{tor} : {torBinaryFound}";
            DebugInformation += $"\n{node} : {nodeBinaryFound}";
            DebugInformation += $"\nDevice Model : {DeviceInfo.Model}";
            DebugInformation += $"\nManufacturer : {DeviceInfo.Manufacturer}";
            DebugInformation += $"\nOperating System Version Number: {DeviceInfo.VersionString}";
            DebugInformation += $"\nPlatform : {DeviceInfo.Platform}";
        }

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
            var yes = await PageDialogService.DisplayAlertAsync("Confirmation", $"Are you sure you want to delete the Blockchain Data ({Settings.DataFolder})?", "Yes", "No");
            if(yes)
            {
                try
                {
                    Directory.Delete(Settings.DataFolder, true);
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                } catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message} ({Settings.DataFolder})");

                    await PageDialogService.DisplayAlertAsync("Error", $"{ex.Message} ({Settings.DataFolder})", "OK");
                }
            }
        }
        
    }
}
