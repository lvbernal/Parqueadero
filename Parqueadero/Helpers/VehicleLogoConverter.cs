using System;
using System.Globalization;
using Xamarin.Forms;

namespace Parqueadero.Helpers
{
    public class VehicleLogoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string vehicle = (string)value;

            if (vehicle != null)
            {
                var logo = vehicle + "Logo";
                return Application.Current.Resources[logo];
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
