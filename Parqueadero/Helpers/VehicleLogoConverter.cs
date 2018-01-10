using System;
using System.Globalization;
using Xamarin.Forms;

namespace Parqueadero.Helpers
{
    public class VehicleLogoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var logo = (string)value + "Logo";
            return Application.Current.Resources[logo];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
