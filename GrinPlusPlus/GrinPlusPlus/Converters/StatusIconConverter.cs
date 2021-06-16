using System;
using System.Globalization;
using Xamarin.Forms;

namespace GrinPlusPlus.Converters
{
    class StatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var status = ((string)value).ToLower();

            if(status.Contains("not finalized"))
            {
                return "baseline_rule_white_20.png";
            } else if (status.Contains("receiving"))
            {
                return "baseline_hourglass_top_white_20.png";
            } else if (status.Contains("sending"))
            {
                return "baseline_hourglass_top_white_20.png";
            }
            else if (status.Contains("received"))
            {
                return "baseline_south_west_white_20.png";
            }
            else if (status.Contains("sent"))
            {
                return "baseline_north_east_white_20.png";
            }
            else if (status.Contains("canceled"))
            {
                return "baseline_not_interested_white_20.png";
            }
            return "baseline_savings_white_20.png"; //coinbase
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
