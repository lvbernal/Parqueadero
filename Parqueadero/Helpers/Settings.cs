using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Parqueadero.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static string ApplicationUrl
        {
            get { return AppSettings.GetValueOrDefault(nameof(ApplicationUrl), string.Empty); }
            set { AppSettings.AddOrUpdateValue(nameof(ApplicationUrl), value); }
        }

        public static string ParkingLotId
        {
            get { return AppSettings.GetValueOrDefault(nameof(ParkingLotId), string.Empty); }
            set { AppSettings.AddOrUpdateValue(nameof(ParkingLotId), value); }
        }

        public static string PrinterUrl
        {
            get { return AppSettings.GetValueOrDefault(nameof(PrinterUrl), string.Empty); }
            set { AppSettings.AddOrUpdateValue(nameof(PrinterUrl), value); }
        }

    }
}
