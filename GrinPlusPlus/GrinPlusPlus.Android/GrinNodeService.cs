using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;

using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using AndroidApp = Android.App.Application;

namespace GrinPlusPlus.Droid
{
    [Service(Exported = false, Name = "com.grinplusplus.mobile.GrinNodeService")]
    public class GrinNodeService : Android.App.Service
    {
        static readonly string TAG = typeof(GrinNodeService).FullName;

        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";

        const int NOTIFICATION_ID = 733100;
        NotificationManager manager;
        bool channelInitialized = false;

        private Java.IO.File libtor;
        private Java.IO.File libgrin;

        private Java.Lang.Process pNode;
        private Java.Lang.Process pTor;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            var librariesPath = PackageManager.GetApplicationInfo(ApplicationInfo.PackageName, PackageInfoFlags.SharedLibraryFiles).NativeLibraryDir;

            libtor = new Java.IO.File(Path.Combine(librariesPath, "libtor.so"));
            libgrin = new Java.IO.File(Path.Combine(librariesPath, "libgrin.so"));

            Preferences.Set("Status", GetStatusLabel(string.Empty));

            RunBackend();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(1);

            var timer = new System.Threading.Timer(async (e) =>
            {
                var label = string.Empty;
                try
                {
                    var nodeStatus = await Service.Node.Instance.Status();
                    
                    label = GetStatusLabel(nodeStatus.SyncStatus);

                    Preferences.Set("HeaderHeight", nodeStatus.HeaderHeight);
                    Preferences.Set("Blocks", nodeStatus.Chain.Height);
                    Preferences.Set("NetworkHeight", nodeStatus.Network.Height);
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, ex.Message);
                    label = GetStatusLabel(string.Empty);
                }
                Preferences.Set("Status", label);
                RegisterForegroundService(label);
            }, null, startTimeSpan, periodTimeSpan);

            RegisterForegroundService("Initializing...");

            return StartCommandResult.Sticky;
        }

        private void RegisterForegroundService(string status)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            // Work has finished, now dispatch anotification to let the user know.
            var notification = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentTitle("Grin Node Status")
                .SetContentText(status)
                .SetSmallIcon(Resource.Drawable.logo)
                .SetOngoing(true)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                .Build();

            StartForeground(NOTIFICATION_ID, notification);
        }

        private string GetStatusLabel(string status)
        {
            switch (status)
            {
                case "FULLY_SYNCED":
                    return "Running";
                case "SYNCING_HEADERS":
                    return "Syncing Headers";
                case "DOWNLOADING_TXHASHSET":
                    return "Downloading State";
                case "PROCESSING_TXHASHSET":
                    return "Validating State";
                case "SYNCING_BLOCKS":
                    return "Syncing Blocks";
                case "NOT_CONNECTED":
                    return "Waiting for Peers";
                default:
                    return "Not Connected";
            }
        }

        void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }

        void RunBackend()
        {
            RunTor();
            RunGrinNode();
        }

        private void RunGrinNode()
        {
            try
            {
                pTor = Java.Lang.Runtime.GetRuntime().Exec(libgrin.AbsolutePath);
            }
            catch (Exception ex)
            {
                RegisterForegroundService(ex.Message);
                RunGrinNode();
            }
        }

        private void RunTor()
        {
            try
            {
                pNode = Java.Lang.Runtime.GetRuntime().Exec(new string[] {
                    libtor.AbsolutePath,
                    "--ControlPort",
                    "3423",
                    "--SocksPort",
                    "3422",
                    "--HashedControlPassword",
                    "16:906248AB51F939ED605CE9937D3B1FDE65DEB4098A889B2A07AC221D8F",
                    "--ignore-missing-torrc",
                    "--quiet"
                });
            }
            catch (Exception ex)
            {
                RegisterForegroundService(ex.Message);
                RunTor();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Preferences.Set("Status", GetStatusLabel(string.Empty));
            StopBackend();
        }

        private void StopBackend()
        {
            if (pNode != null)
            {
                if (pNode.IsAlive)
                {
                    Task task = Task.Factory.StartNew(async () => { await Service.Node.Instance.Shutdown(); });
                    task.Wait();
                    if (pNode.IsAlive)
                    {
                        pNode.DestroyForcibly();
                    }
                }

            }
            if (pTor.IsAlive)
            {
                pTor.DestroyForcibly();
            }
        }
    }
}