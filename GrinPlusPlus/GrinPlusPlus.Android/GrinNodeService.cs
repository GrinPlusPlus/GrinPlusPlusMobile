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

        public Java.IO.File dataFolder { get; private set; }

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

            Preferences.Set("Status", Service.SyncHelpers.GetStatusLabel(string.Empty));
            Preferences.Set("ProgressPercentage", (double)0);

            SetTimer();

            dataFolder = new Java.IO.File(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), ".GrinPP/MAINNET/NODE"));
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action == null)
            {
                RegisterForegroundService(Service.SyncHelpers.GetStatusLabel(string.Empty));
                RunBackend();
            }
            else
            {
                if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
                {
                    Log.Info(TAG, "Stop Service called.");

                    try
                    {
                        Xamarin.Essentials.Platform.CurrentActivity.Finish();
                    }
                    catch (Exception e)
                    {
                        Log.Verbose(TAG, e.Message);
                    }

                    StopBackend();

                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Enabled = false;
                    }

                    StopForeground(true);
                    StopSelf();
                }
                else if (intent.Action.Equals(Constants.ACTION_RESTART_NODE))
                {
                    Log.Info(TAG, "Restart Grin Node called.");
                    StopBackend();
                    RunTor();
                    Task.Run(async () =>
                    {
                        Log.Info(TAG, "Waiting 5 seconds to start the Grin Node again...");
                        await Task.Delay(5000);
                        RunGrinNode();
                    });
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
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += OnTimedEvent;
            timer.Start();
        }

        private async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var label = string.Empty;
            try
            {
                var nodeStatus = await Service.Node.Instance.Status();

                Preferences.Set("ProgressPercentage", Service.SyncHelpers.GetProgressPercentage(nodeStatus));

                label = Service.SyncHelpers.GetStatusLabel(nodeStatus.SyncStatus);

                Preferences.Set("HeaderHeight", nodeStatus.HeaderHeight);
                Preferences.Set("Blocks", nodeStatus.Chain.Height);
                Preferences.Set("NetworkHeight", nodeStatus.Network.Height);
            }
            catch (System.Net.WebException ex)
            {
                Log.Error(TAG, $"Node is not running: {ex.Message}");
                label = Service.SyncHelpers.GetStatusLabel(string.Empty);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"Error Communication: {ex.Message}");
            }

            RegisterForegroundService(label);
            Preferences.Set("Status", label);
        }

        private void RegisterForegroundService(string status)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            // Work has finished, now dispatch anotification to let the user know.
            var notification = new Notification.Builder(AndroidApp.Context, channelId)
                .SetContentTitle("Grin Node")
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
            Log.Info(TAG, "Starting Grin Node...");
            pNode = Java.Lang.Runtime.GetRuntime().Exec(libgrin.AbsolutePath);
            try
            {
                var e = pNode.ExitValue();
                Log.Error(TAG, "ERROR: Grin Node Can't be Started.");
                RunGrinNode();
            }
            catch (Exception)
            {
                Log.Info(TAG, "Grin Node Started.");
            }
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
                Log.Info(TAG, ex.Message);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Preferences.Set("Status", Service.SyncHelpers.GetStatusLabel(string.Empty));

            StopBackend();

            if (timer != null)
            {
                timer.Stop();
            }
        }

        private void StopBackend()
        {
            StopTor();
            StopGrinNode();
            Preferences.Set("Status", Service.SyncHelpers.GetStatusLabel(string.Empty));
        }

        private void StopGrinNode()
        {
            Log.Info(TAG, "Stopping Grin Node...");
            try
            {
                Service.AsyncHelpers.RunSync(Service.Node.Instance.Shutdown);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"Error Stopping Grin Node: {ex.Message}");
            }
            finally
            {
                if (pNode != null)
                {
                    if (pNode.IsAlive)
                    {
                        pNode.DestroyForcibly();
                    }
                }
                Log.Info(TAG, "Grin Node Stopped.");
            }
        }

        private void StopTor()
        {
            Log.Info(TAG, "Stopping Tor...");
            if (pTor != null)
            {
                if (pTor.IsAlive)
                {
                    pTor.DestroyForcibly();
                }
            }
            Java.Lang.Runtime.GetRuntime().Exec(new string[] {
                    "kill",
                    "-9",
                    "libtor.so"
                });
            Log.Info(TAG, "Tor Stopped.");
        }

        private void ResyncNode()
        {
            StopBackend();
            try
            {
                Directory.Delete(dataFolder.AbsolutePath, true);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.Message);
            }
            finally
            {
                RunBackend();
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
            var action = "Run";
            var status = Preferences.Get("Status", string.Empty);
            if (!status.Equals("Not Running"))
            {
                action = "Restart";
            }
            var restartIntent = new Intent(this, GetType());
            restartIntent.SetAction(Constants.ACTION_RESTART_NODE);
            var restartTimerPendingIntent = PendingIntent.GetService(this, 0, restartIntent, 0);

            var builder = new Notification.Action.Builder(null,
                                              action,
                                              restartTimerPendingIntent);

            return builder.Build();
        }

        /// <summary>
		/// Builds a Notification.Action that will instruct the service to resync the node.
		/// </summary>
		/// <returns>The resync node action.</returns>
		Notification.Action BuildResyncNodeAction()
        {
            var status = Preferences.Get("Status", string.Empty);
            if (status.Equals("Not Running"))
            {
                return null;
            }

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
            var status = Preferences.Get("Status", string.Empty);
            if (status.Equals("Not Running"))
            {
                return null;
            }

            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

            var builder = new Notification.Action.Builder(null,
                                                          "EXIT",
                                                          stopServicePendingIntent);
            return builder.Build();
        }
    }
}