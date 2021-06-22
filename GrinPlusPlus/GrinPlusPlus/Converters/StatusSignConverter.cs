using System;
using System.Globalization;
using Xamarin.Forms;

namespace GrinPlusPlus.Converters
{
    class StatusSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var status = ((string)value).ToLower();

            if(status.Contains("not finalized"))
            {
                return "-";
            } else if (status.Contains("receiving"))
            {
                return "+";
            } else if (status.Contains("sending"))
            {
                return "-";
            }
            else if (status.Contains("received"))
            {
                return "+";
            }
            else if (status.Contains("sent"))
            {
                return "-";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
