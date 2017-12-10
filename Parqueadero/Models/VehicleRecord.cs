using System;
using Microsoft.WindowsAzure.MobileServices;
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
        public double Fee
        {
            get
            {
                var fee = BaseFee + TotalAdditionalFee + HelmetsFee;
                return fee > 0 ? fee : 0;
            }
        }

        [Version]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "base_fee")]
        public double BaseFee
        {
            get
            {
                return Constants.GetBaseFee(VehicleType);
            }
        }

        [JsonProperty(PropertyName = "additional_fee")]
        public double AdditionalFee
        {
            get
            {
                return Constants.GetFee(VehicleType);
            }
        }

        [JsonProperty(PropertyName = "helmets_fee")]
        public double HelmetsFee
        {
            get
            {
                return Constants.GetHelmetsFee() * Helmets;
            }
        }

        [JsonProperty(PropertyName = "additional_hours")]
        public int AdditionalHours
        {
            get
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
        }

        [JsonProperty(PropertyName = "total_additional_fee")]
        public double TotalAdditionalFee
        {
            get
            {
                return AdditionalHours * AdditionalFee;
            }
        }
    }
}
