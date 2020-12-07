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
        }
    }
}
