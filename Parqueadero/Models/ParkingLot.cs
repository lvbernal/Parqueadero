using System;
using Parqueadero.Helpers;

namespace Parqueadero.Models
{
    public class ParkingLot
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PrinterUrl { get; set; }

        public int HourToleranceInMinutes { get; set; }

        public double CarBase { get; set; }
        public double CarFee { get; set; }
        public double PickupBase { get; set; }
        public double PickupFee { get; set; }
        public double TruckBase { get; set; }
        public double TruckFee { get; set; }
        public double MotorbikeBase { get; set; }
        public double MotorbikeFee { get; set; }
        public double BikeBase { get; set; }
        public double BikeFee { get; set; }
        public double HelmetsBase { get; set; }

        public ParkingLot()
        {
            Id = "PARKINGLOT_ID";
            Name = "PARKINGLOT_NAME";
            PrinterUrl = "PRINTER_URL";

            HourToleranceInMinutes = 10;

            CarBase = 2500;
            CarFee = 2500;
            PickupBase = 2500;
            PickupFee = 2500;
            TruckBase = 3000;
            TruckFee = 3000;
            MotorbikeBase = 1500;
            MotorbikeFee = 500;
            BikeBase = 1000;
            BikeFee = 0;
            HelmetsBase = 500;
        }

        public double GetBaseFee(string vehicleType)
        {
            switch (vehicleType)
            {
                case Constants.Car:
                    return CarBase;
                case Constants.Pickup:
                    return PickupBase;
                case Constants.Truck:
                    return TruckBase;
                case Constants.Motorbike:
                    return MotorbikeBase;
                case Constants.Bike:
                    return BikeBase;
            }

            return 0;
        }

        public double GetFee(string vehicleType)
        {
            switch (vehicleType)
            {
                case Constants.Car:
                    return CarFee;
                case Constants.Pickup:
                    return PickupFee;
                case Constants.Truck:
                    return TruckFee;
                case Constants.Motorbike:
                    return MotorbikeFee;
                case Constants.Bike:
                    return BikeFee;
            }

            return 0;
        }

        public double GetHelmetsFee()
        {
            return HelmetsBase;
        }

        public FeeCheckOutResult CalculateFeeDetails(VehicleRecord vehicle)
        {
            var baseFee = GetBaseFee(vehicle.VehicleType);
            var additionalHours = CalculateAdditionalHours(vehicle);
            var additionalFee = additionalHours * GetFee(vehicle.VehicleType);
            var helmetsFee = CalculateHelmetsFee(vehicle);

            var totalFee = baseFee + additionalFee + helmetsFee;
            totalFee = totalFee > 0 ? totalFee : 0;

            return new FeeCheckOutResult()
            {
                BaseFee = baseFee,
                AdditionalHours = additionalHours,
                AdditionalFee = additionalFee,
                HelmetsFee = helmetsFee,
                TotalFee = totalFee
            };
        }

        private int CalculateAdditionalHours(VehicleRecord vehicle)
        {
            var difference = vehicle.CheckOut - vehicle.CheckIn;
            var hours = difference.Hours;
            var minutes = difference.Minutes;

            if (hours > 0 && minutes <= HourToleranceInMinutes)
            {
                hours--;
            }

            return hours > 0 ? hours : 0;
        }

        private double CalculateHelmetsFee(VehicleRecord vehicle)
        {
            return GetHelmetsFee() * vehicle.Helmets;
        }
    }
}
