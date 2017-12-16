using System;
using System.Globalization;
using Xamarin.Forms;

namespace Parqueadero.Helpers
{
    public class VehicleOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (bool)value ? "selectedVehicleColor" : "unselectedVehicleColor";
            return Application.Current.Resources[color];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
