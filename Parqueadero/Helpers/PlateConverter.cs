using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Parqueadero.Helpers
{
    public class PlateConverter : IValueConverter
    {
        private Regex rx = new Regex("[^a-zA-Z0-9]");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return rx.Replace((string)value, "").ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
