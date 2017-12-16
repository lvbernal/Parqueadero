using System;
namespace Parqueadero.Models
{
    public class FeeCheckOutResult
    {
        public double BaseFee { get; set; }
        public int AdditionalHours { get; set; }
        public double AdditionalFee { get; set; }
        public double HelmetsFee { get; set; }
        public double TotalFee { get; set; }
    }
}
