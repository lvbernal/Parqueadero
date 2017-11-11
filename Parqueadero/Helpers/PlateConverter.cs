using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Parqueadero.Helpers
{
    public class PlateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			var plate = (string)value;

            Regex rx = new Regex("[^a-zA-Z0-9]");
            plate = rx.Replace(plate, "").ToUpper();

            return plate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
