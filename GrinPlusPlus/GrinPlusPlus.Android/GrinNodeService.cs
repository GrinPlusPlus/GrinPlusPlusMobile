using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using GrinPlusPlus.Droid.Classes;
using System;

using AndroidApp = Android.App.Application;

namespace GrinPlusPlus.Droid
{
    [Service(Exported = false, Name = "com.grinplusplus.mobile.GrinNodeService")]
    public class GrinNodeService : Android.App.Service
    {
        static readonly string TAG = typeof(GrinNodeService).FullName;

        System.Timers.Timer timer;

        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";

        static string nativeLibraryDir;

        NotificationManager manager;
        bool channelInitialized = false;
        
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            nativeLibraryDir = PackageManager.GetApplicationInfo(ApplicationInfo.PackageName, PackageInfoFlags.SharedLibraryFiles).NativeLibraryDir;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action == null)
            {
                InitializeNodeService();

                if(NodeControl.IsNodeRunning())
                {
                    RegisterForegroundService("Initializing Node...");
                    SetNodeTimer();
                } else
                {
                    RegisterForegroundService("Not Running");
                }
            }
            else
            {
                try
                {
                    NodeControl.StopNode();
                }
                catch { }
                try
                {
                    NodeControl.StopTor();
                }
                catch { }

                if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
                {
                    Log.Info(TAG, "Stop Service called.");
                    
                    try
                    {
                        if (timer != null)
                        {
                            timer.Stop();
                            timer.Enabled = false;
                        }

                        Xamarin.Essentials.Platform.CurrentActivity.FinishAffinity();
                    }
                    catch (Exception e)
                    {
                        Log.Verbose(TAG, e.Message);
                    }

                    StopForeground(true);
                    StopSelf();

                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                else if (intent.Action.Equals(Constants.ACTION_RESTART_NODE))
                {
                    Log.Info(TAG, "Restart Grin Node called.");

                    InitializeNodeService();
                }
            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

        private void InitializeNodeService()
        {
            try
            {
                Log.Verbose(TAG, "Starting Tor...");
                NodeControl.StartTor(nativeLibraryDir);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.Message);
            }

            try
            {
                var peersFolder = Xamarin.Essentials.Preferences.Get("PeersFolder", "");
                NodeControl.DeleteNodeDataFolder(peersFolder);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.Message);
            }

            try
            {
                Log.Verbose(TAG, "Starting Node...");
                NodeControl.StartNode(nativeLibraryDir);
            }
            catch (Exception ex)
            {
                try
                {
                    if (NodeControl.IsTorRunning())
                    {
                        NodeControl.StopTor();
                    }
                } catch { }

                Log.Error(TAG, ex.Message);
            }
        }

        private void SetNodeTimer()
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnNodeTimedEvent;
            timer.Start();
        }

        private async void OnNodeTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("IsTorRunning", NodeControl.IsTorRunning());
            Xamarin.Essentials.Preferences.Set("IsNodeRunning", NodeControl.IsNodeRunning());

            try
            {
                var nodeStatus = await Service.Node.Instance.Status();
                Xamarin.Essentials.Preferences.Set("HeaderHeight", nodeStatus.HeaderHeight);
                Xamarin.Essentials.Preferences.Set("Blocks", nodeStatus.Chain.Height);
                Xamarin.Essentials.Preferences.Set("NetworkHeight", nodeStatus.Network.Height);

                var percentage = Service.SyncHelpers.GetProgressPercentage(nodeStatus);
                Xamarin.Essentials.Preferences.Set("ProgressPercentage", percentage);

                var label = Service.SyncHelpers.GetStatusLabel(nodeStatus.SyncStatus);
                Xamarin.Essentials.Preferences.Set("Status", label);

                if (!label.Equals("Not Running") && !label.Equals("Waiting for Peers") && !label.Equals("Running"))
                {
                    try
                    {
                        label = $"{label} ({string.Format($"{ percentage * 100:F}")}%)";
                    } catch { }
                }

                RegisterForegroundService(label);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "============================ERROR===============================================");
                Log.Error(TAG, $"{ex.Message}");
                if (!NodeControl.IsNodeRunning())
                {
                    Log.Error(TAG, $"ERROR: Grin local Node is not Running.");
                    Xamarin.Essentials.Preferences.Set("Status", "Not Running");
                    RegisterForegroundService("Not Running");
                }
                Log.Error(TAG, "============================ERROR===============================================");
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Xamarin.Essentials.Preferences.Set("Status", Service.SyncHelpers.GetStatusLabel(string.Empty));

            try
            {
                NodeControl.StopNode();
            } catch { }
            try
            {
                NodeControl.StopTor();
            }
            catch { }
            
            if (timer != null)
            {
                timer.Stop();
            }
        }

        private void RegisterForegroundService(string status)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            // Work has finished, now dispatch a notification to let the user know.
            var notification = new Notification.Builder(AndroidApp.Context, channelId)
                .SetContentTitle("Node Status")
                .SetContentText(status)
                .SetSmallIcon(Resource.Drawable.ic_stat_grin)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .AddAction(BuildRestartNodeAction())
                .AddAction(BuildStopServiceAction())
                .Build();

            // Enlist this instance of the service as a foreground service
            StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            var channelNameJava = new Java.Lang.String(channelName);
            var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.None)
            {
                Description = channelDescription,
                Importance = NotificationImportance.Low
            };
            channel.EnableVibration(false);
            channel.EnableLights(false);
            channel.SetSound(null, null);
            channel.EnableVibration(false);
            
            manager.CreateNotificationChannel(channel);

            channelInitialized = true;
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

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        /// <summary>
		/// Builds a Notification.Action that will instruct the service to restart the node.
		/// </summary>
		/// <returns>The restart node action.</returns>
		Notification.Action BuildRestartNodeAction()
        {
            var action = "Run";
            
            if (NodeControl.IsNodeRunning()) return null;
            
            var restartIntent = new Intent(this, GetType());
            restartIntent.SetAction(Constants.ACTION_RESTART_NODE);
            var restartTimerPendingIntent = PendingIntent.GetService(this, 0, restartIntent, PendingIntentFlags.Immutable);

            var builder = new Notification.Action.Builder(null,
                                              action,
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
            if (!NodeControl.IsNodeRunning()) return null;

            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, PendingIntentFlags.Immutable);

            var builder = new Notification.Action.Builder(null,
                                                          "Close",
                                                          stopServicePendingIntent);
            return builder.Build();
        }
    }
}