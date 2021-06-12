using System;
using System.Globalization;
using Xamarin.Forms;

namespace GrinPlusPlus.Converters
{
    class AvailabilityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            return (bool)value == true ? "Green" : "Orange" ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
