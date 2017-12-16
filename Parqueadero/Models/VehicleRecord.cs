using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Parqueadero.Helpers;

namespace Parqueadero.Models
{
    public class VehicleRecord
    {
        public const string Car = "car";
        public const string Pickup = "pickup";
        public const string Truck = "truck";
        public const string Motorbike = "motorbike";
        public const string Bike = "bike";

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "parkinglot")]
        public string ParkingLotId { get; set; }

        [JsonProperty(PropertyName = "plate")]
        public string Plate { get; set; }

        [JsonProperty(PropertyName = "vehicle")]
        public string VehicleType { get; set; }

        [JsonProperty(PropertyName = "checkin")]
        public DateTime CheckIn { get; set; }

        [JsonProperty(PropertyName = "checkout")]
        public DateTime CheckOut { get; set; }

        [JsonProperty(PropertyName = "helmets")]
        public int Helmets { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Done { get; set; }

        [JsonProperty(PropertyName = "fee")]
        public double Fee { get; set; }

        [Version]
        public string Version { get; set; }

        public double CalculateFee()
        {
            var _fee = GetBaseFee() + GetTotalAdditionalFee() + GetHelmetsFee();
            Fee = _fee > 0 ? _fee : 0;
            return Fee;
        }

        public double GetBaseFee()
        {
            return Constants.GetBaseFee(VehicleType);
        }

        public double GetAdditionalFee()
        {
            return Constants.GetFee(VehicleType);
        }

        public double GetHelmetsFee()
        {
            return Constants.GetHelmetsFee() * Helmets;
        }

        public int GetAdditionalHours()
        {
            var difference = CheckOut - CheckIn;
            var hours = difference.Hours;
            var minutes = difference.Minutes;

            if (hours > 0 && minutes <= Constants.HourToleranceInMinutes)
            {
                hours--;
            }

            return hours > 0 ? hours : 0;
        }

        public double GetTotalAdditionalFee()
        {
            return GetAdditionalHours() * GetAdditionalFee();
        }
    }
}
