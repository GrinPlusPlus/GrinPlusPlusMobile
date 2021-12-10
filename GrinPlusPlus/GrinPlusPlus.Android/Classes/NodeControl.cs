using Android.Util;
using System;
using System.IO;


namespace GrinPlusPlus.Droid.Classes
{
    public static class NodeControl
    {
        static readonly string TAG = typeof(GrinNodeService).FullName;

        public static Java.Lang.Process pNode;
        public static Java.Lang.Process pTor;

        public static void StartNode(string nativeLibraryDirectory)
        {
            string DBLockFile = Path.Combine(Path.Combine(new Java.IO.File(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), ".GrinPP/MAINNET/")).AbsolutePath, "DB", "CHAIN", "LOCK"), "DB", "CHAIN", "LOCK");
            Java.IO.File libgrin = new Java.IO.File(Path.Combine(nativeLibraryDirectory, "libgrin.so"));

            Log.Info(TAG, "Starting Grin Node...");

            if (File.Exists(DBLockFile))
            {
                File.Delete(DBLockFile);
            }

            try
            {
                pNode = Java.Lang.Runtime.GetRuntime().Exec(libgrin.AbsolutePath);
                var e = pNode.ExitValue();
                Log.Error(TAG, "Grin Node can't be Started.");
            }
            catch (Exception)
            {
                Log.Info(TAG, "Grin Node Started.");
            }
        }

        public static void StopNode()
        {
            Log.Info(TAG, "Stopping Grin Node...");
            try
            {
                Service.AsyncHelpers.RunSync(Service.Node.Instance.Shutdown);
            }
            catch (Exception ex)
            {
                Java.Lang.Runtime.GetRuntime().Exec(new string[] {
                    "killall",
                    "-9",
                    "libgrin.so"
                });

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

        public static void StartTor(string nativeLibraryDirectory)
        {
            Java.IO.File libtor = new Java.IO.File(Path.Combine(nativeLibraryDirectory, "libtor.so"));

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
                var e = pTor.ExitValue();
                Log.Error(TAG, "Tor can't be Started.");
            }
            catch (Exception)
            {
                Log.Info(TAG, "Tor Started.");
            }
        }

        public static void StopTor()
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
                    "killall",
                    "-9",
                    "libtor.so"
                }).WaitFor();

            Log.Info(TAG, "Tor Stopped.");
        }

        public static bool IsNodeRunning()
        {
            return IsBinaryRunning("libgrin.so");
        }

        public static bool IsTorRunning()
        {
            return IsBinaryRunning("libtor.so");
        }

        public static bool DeleteNodeDataFolder(string dataFolder)
        {
            if (IsTorRunning())
            {
                StopTor();
            }
            if (IsNodeRunning())
            {
                StopNode();
            }

            try
            {
                Directory.Delete(dataFolder, true);

                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, ex.Message);
                return false;
            }
        }

        static bool IsBinaryRunning(string binary)
        {
            try
            {
                Java.Lang.Process process = Java.Lang.Runtime.GetRuntime().Exec("ps -A");

                Java.IO.BufferedReader reader = new Java.IO.BufferedReader(
                        new Java.IO.InputStreamReader(process.InputStream));

                int read;
                char[] buffer = new char[4096];
                Java.Lang.StringBuffer output = new Java.Lang.StringBuffer();
                while ((read = reader.Read(buffer)) > 0)
                {
                    output.Append(buffer, 0, read);
                }
                reader.Close();

                process.WaitFor();

                return output.ToString().Contains($"{binary}");
            }
            catch (IOException e)
            {
                Log.Error(TAG, e.ToString());
            }
            catch (Java.Lang.InterruptedException e)
            {
                Log.Error(TAG, e.ToString());
            }

            return false;
        }
    }
}
