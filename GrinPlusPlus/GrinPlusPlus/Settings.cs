using System.Globalization;
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

        public static string CurrentPage
        {
            get => Preferences.Get(nameof(CurrentPage), "InitPage");
            set => Preferences.Set(nameof(CurrentPage), value);
        }

        public static string CurrentCultureInfo
        {
            get => Preferences.Get(nameof(CurrentCultureInfo), new CultureInfo("en-US").Name);
            set => Preferences.Set(nameof(CurrentCultureInfo), value);
        }

        public static string GrinChckAPIURL
        {
            get => Preferences.Get(nameof(GrinChckAPIURL), "http://grinchck.ahcbagldgzdpa74g2mh74fvk5zjzpfjbvgqin6g3mfuu66tynv2gkiid.onion/check/");
            set => Preferences.Set(nameof(GrinChckAPIURL), value);
        }

        public static int Confirmations
        {
            get => Preferences.Get(nameof(Confirmations), 10);
            set => Preferences.Set(nameof(Confirmations), value);
        }

        public static int MinimumPeers
        {
            get => Preferences.Get(nameof(MinimumPeers), 10);
            set => Preferences.Set(nameof(MinimumPeers), value);
        }

        public static int MaximumPeers
        {
            get => Preferences.Get(nameof(MaximumPeers), 60);
            set => Preferences.Set(nameof(MaximumPeers), value);
        }

        public static class Node
        {
            public static string Status
            {
                get => Preferences.Get(nameof(Status), "Not Connected");
                set => Preferences.Set(nameof(Status), value);
            }

            public static int HeaderHeight
            {
                get => Preferences.Get(nameof(HeaderHeight), 0);
                set => Preferences.Set(nameof(HeaderHeight), value);
            }

            public static int Blocks
            {
                get => Preferences.Get(nameof(Blocks), 0);
                set => Preferences.Set(nameof(Blocks), value);
            }

            public static int NetworkHeight
            {
                get => Preferences.Get(nameof(NetworkHeight), 0);
                set => Preferences.Set(nameof(NetworkHeight), value);
            }

            public static double ProgressPercentage
            {
                get => Preferences.Get(nameof(ProgressPercentage), (double)0);
                set => Preferences.Set(nameof(ProgressPercentage), value);
            }
        }
    }
}
