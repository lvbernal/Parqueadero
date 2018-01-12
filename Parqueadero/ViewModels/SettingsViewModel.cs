using System;
using Parqueadero.Helpers;

namespace Parqueadero.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public string ParkingLotId
        {
            get { return Settings.ParkingLotId; }
            set
            {
                Settings.ParkingLotId = value;
                NotifyPropertyChanged();
            }
        }

        public string PrinterUrl
        {
            get { return Settings.PrinterUrl; }
            set
            {
                Settings.PrinterUrl = value;
                NotifyPropertyChanged();
            }
        }
    }
}
