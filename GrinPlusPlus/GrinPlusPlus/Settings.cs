using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GrinPlusPlus
{
    public static class Settings
    {
        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }

        public static string NodeHost
        {
            get => Preferences.Get(nameof(NodeHost), "localhost");
            set => Preferences.Set(nameof(NodeHost), value);
        }

        public static bool IsLoggedIn
        {
            get => Preferences.Get(nameof(IsLoggedIn), false);
            set => Preferences.Set(nameof(IsLoggedIn), value);
        }
    }
}
