using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

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
    }
}
