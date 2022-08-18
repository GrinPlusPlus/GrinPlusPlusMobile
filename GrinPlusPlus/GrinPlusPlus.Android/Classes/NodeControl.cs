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

            if (!libgrin.CanExecute())
            {
                libgrin.SetExecutable(true);
            }
            
            Log.Info(TAG, "Starting Grin Node...");
            
            if (File.Exists(DBLockFile))
            {
                File.Delete(DBLockFile);
            }

            try
            {
                Java.Lang.ProcessBuilder builder = new Java.Lang.ProcessBuilder();
                pNode = builder.Command(libgrin.AbsolutePath).InheritIO().Start();

                Log.Info(TAG, "============================================================================");
                Log.Info(TAG, "Grin Node Started.");
                Log.Info(TAG, "============================================================================");
            } catch (Exception ex)
            {
                Log.Error(TAG, "============================================================================");
                Log.Error(TAG, "ERROR: Grin Node can't be started.");
                Log.Error(TAG, ex.ToString());
                Log.Error(TAG, "============================================================================");
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
            return GetProcessId(pNode) != -1;
        }

        public static bool IsTorRunning()
        {
            return GetProcessId(pTor) != -1;
        }

        public static bool DeleteNodeDataFolder()
        {
            string dataFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), ".GrinPP/MAINNET/NODE");

            try
            {
                if (NodeControl.IsNodeRunning())
                {
                    NodeControl.StopNode();
                }
            } catch (Exception ex)
            {
                Log.Error(TAG, ex.Message);
            }
            
            try
            {
                Directory.Delete(dataFolder, true);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.Message);
                return false;
            }
        }

        static int GetProcessId(Java.Lang.Process p)
        {
            int pid;

            if (p == null) return -1;

            try
            {
                Java.Lang.Reflect.Field f = p.Class.GetDeclaredField("pid");
                f.Accessible = true;
                pid = f.GetInt(p);
                f.Accessible = false;
            }
            catch (Exception)
            {
                try
                {
                    Java.Util.Regex.Matcher m = Java.Util.Regex.Pattern.Compile("pid=(\\d+)").Matcher(p.ToString());
                    pid = m.Find() ? Java.Lang.Integer.ParseInt(m.Group(1)) : -1;
                }
                catch (Exception)
                {
                    pid = -1;
                }
            }
            return pid;
        }
    }
}
