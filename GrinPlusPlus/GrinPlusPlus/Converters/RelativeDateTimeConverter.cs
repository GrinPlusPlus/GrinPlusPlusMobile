using System;
using System.Globalization;
using Xamarin.Forms;

namespace GrinPlusPlus.Converters
{
    class RelativeDateTimeConverter : IValueConverter
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            return DoConversion(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string DoConversion(object value)
        {
            var postedData = (DateTime)value;

            var ts = new TimeSpan(DateTime.Now.Ticks - postedData.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);
            if (delta < 1 * MINUTE)
            {
                if (ts.Seconds < 0)
                {
                    return "sometime ago";
                }
                return ts.Seconds == 1 ? "One second ago" : ts.Seconds + " seconds ago";
            }

            if (delta < 2 * MINUTE)
                return "A minute ago";

            if (delta < 45 * MINUTE)
            {
                if (ts.Seconds < 0)
                {
                    return "sometime ago";
                }
                return ts.Minutes + " minutes ago";
            }

            if (delta <= 90 * MINUTE)
                return "An hour ago";

            if (delta < 24 * HOUR)
            {
                if (ts.Hours < 0)
                {
                    return "sometime ago";
                }

                if (ts.Hours == 1)
                    return "1 hour ago";

                return ts.Hours + " hours ago";
            }

            if (delta < 48 * HOUR)
                return $"Yesterday at {postedData:t}";

            if (delta < 30 * DAY)
            {
                if (ts.Days == 1)
                    return "1 day ago";

                return ts.Days + " days ago";
            }


            if (delta < 12 * MONTH)
            {
                int months = (int)(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = (int)(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }
}
