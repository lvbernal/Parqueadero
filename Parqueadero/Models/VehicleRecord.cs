using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace Parqueadero.Models
{
    public class VehicleRecord
    {
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

        [JsonProperty(PropertyName = "base_fee")]
        public double BaseFee { get; set; }

        [JsonProperty(PropertyName = "additional_fee")]
        public double AdditionalFee { get; set; }

        [JsonProperty(PropertyName = "helmets_fee")]
		public double HelmetsFee { get; set; }

        [JsonProperty(PropertyName = "additional_hours")]
        public int AdditionalHours { get; set; }

        [JsonProperty(PropertyName = "total_additional_fee")]
        public double TotalAdditionalFee { get; set; }
    }
}
