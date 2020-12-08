using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;

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

        private System.Timers.Timer timer;

        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";

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

            SetTimer();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action == null)
            {
                Log.Info(TAG, "Starting Grin Node.");
                RegisterForegroundService("Initializing node");
                RunBackend();
            }
            else
            {
                if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
                {
                    Log.Info(TAG, "Stopping Grin Node...");
                    StopBackend();
                    Log.Info(TAG, "Grin Node stopped.");
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Enabled = false;
                    }
                    Preferences.Set("Status", GetStatusLabel(string.Empty));
                    StopForeground(true);
                    StopSelf();
                }
                else if (intent.Action.Equals(Constants.ACTION_RESTART_NODE))
                {
                    Log.Info(TAG, "Restarting Grin Node...");
                    StopBackend();
                    RunBackend();
                }
                else if (intent.Action.Equals(Constants.ACTION_RESYNC_NODE))
                {
                    Log.Info(TAG, "Resync Grin Node called.");
                    ResyncNode();
                }
            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

        private void SetTimer()
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimedEvent;
            Task.Delay(10000).ContinueWith(t => timer.Start());
        }

        private async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
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
                Log.Error(TAG, $"Error Communication: {ex.Message}");
                label = GetStatusLabel(string.Empty);
            }
            Preferences.Set("Status", label);
            RegisterForegroundService(label);
        }

        private void RegisterForegroundService(string status)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            // Work has finished, now dispatch anotification to let the user know.
            var notification = new Notification.Builder(AndroidApp.Context, channelId)
                .SetContentTitle("Grin Full Node")
                .SetContentText(status)
                .SetSmallIcon(Resource.Drawable.logo)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .AddAction(BuildRestartNodeAction())
                .AddAction(BuildResyncNodeAction())
                .AddAction(BuildStopServiceAction())
                .Build();

            // Enlist this instance of the service as a foreground service
            StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        private static string GetStatusLabel(string status)
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
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.None)
                {
                    Description = channelDescription,
                };
                channel.EnableVibration(false);
                channel.EnableLights(false);
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
            Task.Run(async () =>
            {
                try
                {
                    await Service.Node.Instance.Shutdown();
                }
                catch (Exception)
                {
                    Log.Info(TAG, "No Grin Node process running.");
                }
                finally
                {
                    try
                    {
                        pNode = Java.Lang.Runtime.GetRuntime().Exec(libgrin.AbsolutePath);
                        Log.Info(TAG, "Grin Node Started.");
                    }
                    catch (Exception ex)
                    {
                        RegisterForegroundService(ex.Message);
                        Log.Error(TAG, $"Error Stopping Grin Node: {ex.Message}");
                    }
                }
            });
        }

        private void RunTor()
        {
            try
            {
                pTor = Java.Lang.Runtime.GetRuntime().Exec(new string[] {
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
            if (timer != null)
            {
                timer.Stop();
            }
        }

        private void StopBackend()
        {
            if (pNode != null)
            {
                if (pNode.IsAlive)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Service.Node.Instance.Shutdown();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(TAG, $"Error Stopping Grin Node: {ex.Message}");
                        }
                        finally
                        {
                            if (pNode.IsAlive)
                            {
                                pNode.DestroyForcibly();
                            }
                        }

                    });
                }
            }
            if (pTor != null)
            {
                if (pTor.IsAlive)
                {
                    pTor.DestroyForcibly();
                }
            }
        }

        private void ResyncNode()
        {
            if (pNode != null)
            {
                if (pNode.IsAlive)
                {

                    Task.Run(async () =>
                    {
                        try
                        {
                            await Service.Node.Instance.Resync();
                            Log.Info(TAG, "Resync signal sent to Grin Node.");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(TAG, $"Error Trying to Resync Grin Node: {ex.Message}");
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Builds a PendingIntent that will display the main activity of the app. This is used when the 
        /// user taps on the notification; it will take them to the main activity of the app.
        /// </summary>
        /// <returns>The content intent.</returns>
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        /// <summary>
		/// Builds a Notification.Action that will instruct the service to restart the node.
		/// </summary>
		/// <returns>The restart node action.</returns>
		Notification.Action BuildRestartNodeAction()
        {
            var restartIntent = new Intent(this, GetType());
            restartIntent.SetAction(Constants.ACTION_RESTART_NODE);
            var restartTimerPendingIntent = PendingIntent.GetService(this, 0, restartIntent, 0);

            var builder = new Notification.Action.Builder(null,
                                              "Restart",
                                              restartTimerPendingIntent);

            return builder.Build();
        }

        /// <summary>
		/// Builds a Notification.Action that will instruct the service to resync the node.
		/// </summary>
		/// <returns>The resync node action.</returns>
		Notification.Action BuildResyncNodeAction()
        {
            var resyncNodeIntent = new Intent(this, GetType());
            resyncNodeIntent.SetAction(Constants.ACTION_RESYNC_NODE);
            var restartTimerPendingIntent = PendingIntent.GetService(this, 0, resyncNodeIntent, 0);

            var builder = new Notification.Action.Builder(null,
                                              "(RE)Synchronize",
                                              restartTimerPendingIntent);

            return builder.Build();
        }

        /// <summary>
		/// Builds the Notification.Action that will allow the user to stop the service via the
		/// notification in the status bar
		/// </summary>
		/// <returns>The stop service action.</returns>
		Notification.Action BuildStopServiceAction()
        {
            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

            var builder = new Notification.Action.Builder(null,
                                                          "STOP",
                                                          stopServicePendingIntent);
            return builder.Build();
        }
    }
}