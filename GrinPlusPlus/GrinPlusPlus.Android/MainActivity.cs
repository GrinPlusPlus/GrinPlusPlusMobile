using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Plugin.Fingerprint;
using Prism;
using Prism.Ioc;

namespace GrinPlusPlus.Droid
{
    [Activity(Label = "Grin++", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossFingerprint.SetCurrentActivityResolver(() => this);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);

            Xamarin.Essentials.Preferences.Set("IsLoggedIn", false);
            Xamarin.Essentials.Preferences.Set("Status", Service.SyncHelpers.GetStatusLabel(string.Empty));
            Xamarin.Essentials.Preferences.Set("ProgressPercentage", (float)0);
            Xamarin.Essentials.Preferences.Set("DataFolder", System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".GrinPP/MAINNET/NODE"));
            Xamarin.Essentials.Preferences.Set("PeersFolder", System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".GrinPP/MAINNET/NODE/DB/PEERS"));
            Xamarin.Essentials.Preferences.Set("LogsFolder", System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".GrinPP/MAINNET/LOGS"));
            Xamarin.Essentials.Preferences.Set("BackendFolder", System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".GrinPP/MAINNET/"));

            Xamarin.Essentials.Preferences.Set("NativeLibraryDir", PackageManager.GetApplicationInfo(ApplicationInfo.PackageName, PackageInfoFlags.SharedLibraryFiles).NativeLibraryDir);

            LoadApplication(new App(new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            // always call the base implementation!
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}

