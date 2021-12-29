using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Xamarin.Essentials;


namespace GrinPlusPlus.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private bool _isLogged = Settings.IsLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLogged;
            set => SetProperty(ref _isLogged, value);
        }

        private int _confirmations = Settings.Confirmations;
        public int Confirmations
        {
            get => _confirmations;
            set => SetProperty(ref _confirmations, value);
        }

        private int _minimumPeers = Settings.MinimumPeers;
        public int MinimumPeers
        {
            get => _minimumPeers;
            set => SetProperty(ref _minimumPeers, value);
        }

        private int _maximumPeers = Settings.MaximumPeers;
        public int MaximumPeers
        {
            get => _maximumPeers;
            set => SetProperty(ref _maximumPeers, value);
        }

        private string _grinchckurl = Settings.GrinChckAPIURL;
        public string GrinChckAPIUrl
        {
            get => _grinchckurl.Trim();
            set => SetProperty(ref _grinchckurl, value);
        }

        public SettingsPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }

        public DelegateCommand UpdateNodeSettingsCommand => new DelegateCommand(UpdateNodeSetting);
        async void UpdateNodeSetting()
        {
            IsLoggedIn = Settings.IsLoggedIn;
            try
            {
                await DataProvider.UpdateNodeSettings(MinimumPeers, MaximumPeers, Confirmations).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public DelegateCommand ExportLogsButtonClickedCommand => new DelegateCommand(ExportLogsButtonClicked);
        async void ExportLogsButtonClicked()
        {
            string zipPath = Path.Combine(Settings.BackendFolder, "logs.zip");
            try
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                if(IsLoggedIn)
                {
                    ZipFile.CreateFromDirectory(Settings.LogsFolder, zipPath);
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = "Logs",
                        File = new ShareFile(zipPath)
                    });
                }
                else
                {
                    using (FileStream fs = new FileStream(zipPath, FileMode.Create))
                    using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        var entryFileName = Path.Combine(Settings.LogsFolder, "Node.log");
                        arch.CreateEntryFromFile(entryFileName, "Node.log");
                    }
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            } finally
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
            }
        }
    }
}
