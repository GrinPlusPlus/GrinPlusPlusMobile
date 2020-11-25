using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace GrinPlusPlus.Droid
{
	[Activity(Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class SplashActivity : AppCompatActivity
	{
		private CancellationTokenSource cancellation;

		private string librariesPath;

		private string nodeStatus;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Splash);
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			librariesPath = PackageManager.GetApplicationInfo(ApplicationInfo.PackageName, PackageInfoFlags.SharedLibraryFiles).NativeLibraryDir;
			cancellation = new CancellationTokenSource();

			FixPermissions(librariesPath);

			RunBackend();
		}

		protected override void OnResume()
		{
			base.OnResume();

			CancellationTokenSource cts = this.cancellation;

			Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(3), () => 
			{
				if (cts.IsCancellationRequested) return false;
				RunOnUiThread(async () =>
				{
					if(string.IsNullOrEmpty(nodeStatus))
                    {
						nodeStatus = (await GrinPlusPlus.Service.Node.Instance.Status()).SyncStatus;
						if (!string.IsNullOrEmpty(nodeStatus))
						{
							StartActivity(new Intent(Application.Context, typeof(MainActivity)));
						}
					}
				});
				return true;
			});
		}


		protected override void OnStop()
		{
			base.OnStop();

			Interlocked.Exchange(ref cancellation, new CancellationTokenSource()).Cancel();
		}

		protected void FixPermissions(string librariesPath)
		{
			Java.IO.File libraries = new Java.IO.File(librariesPath);

			if (libraries.Exists())
			{
				foreach (Java.IO.File library in libraries.ListFiles())
				{
					if ((new string[2] {
						"libtor.so",
						"libgrin.so"
					}).Contains(library.Name))
					{
						library.SetReadable(true);
						library.SetWritable(true, true);
						library.SetExecutable(true);
					}
				}
			}
		}

		protected void RunBackend()
		{
			Java.Lang.Runtime.GetRuntime().Exec(Path.Combine(librariesPath, "libgrin.so"));

			Java.Lang.Runtime.GetRuntime().Exec(new string[] {
				Path.Combine(librariesPath, "libtor.so"),
				"--quiet",
				"--SOCKSPort",
				"3422",
				"--ControlPort",
				"3423",
				"--HashedControlPassword",
				"16:906248AB51F939ED605CE9937D3B1FDE65DEB4098A889B2A07AC221D8F"
			});
		}
	}
}