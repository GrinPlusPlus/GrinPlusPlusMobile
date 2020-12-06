using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using System.IO;
using System.Threading.Tasks;
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
        }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(TAG, "GrinNode started");

            RunBackend();

            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            // Work has finished, now dispatch anotification to let the user know.
            /*NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentTitle("Grin Node")
                .SetContentText("A full Grin node is Running.")
                .SetSmallIcon(Resource.Drawable.logo)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            var notification = builder.Build();
            manager.Notify(NOTIFICATION_ID, notification);*/

            return StartCommandResult.Sticky;
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
            Java.Lang.Runtime.GetRuntime().Exec(libgrin.AbsolutePath);

            Java.Lang.Runtime.GetRuntime().Exec(new string[] {
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

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopBackend();   
        }

        private void StopBackend()
        {
            Task task = Task.Factory.StartNew(async () => { await Service.Node.Instance.Shutdown(); });
            task.Wait();
        }
    }
}