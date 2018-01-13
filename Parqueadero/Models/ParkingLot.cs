using System;
using Parqueadero.Helpers;

namespace Parqueadero.Models
{
    public class ParkingLot
    {
        public static int HourToleranceInMinutes { get; set; }
        public static double CarBase { get; set; }
        public static double CarFee { get; set; }
        public static double PickupBase { get; set; }
        public static double PickupFee { get; set; }
        public static double TruckBase { get; set; }
        public static double TruckFee { get; set; }
        public static double MotorbikeBase { get; set; }
        public static double MotorbikeFee { get; set; }
        public static double BikeBase { get; set; }
        public static double BikeFee { get; set; }
        public static double HelmetsBase { get; set; }

        private ParkingLot()
        {
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

        public static double GetBaseFee(string vehicleType)
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
                default:
                    return 0;
            }
        }

        public static double GetFee(string vehicleType)
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
                default:
                    return 0;
            }
        }

        public static double GetHelmetsFee()
        {
            return HelmetsBase;
        }

        public static VehicleRecord AddCheckInInfoForVehicle(VehicleRecord vehicle)
        {
            vehicle.CheckIn = DateTime.Now.ToLocalTime();
            vehicle.ParkingLotId = Settings.ParkingLotId;
            vehicle.BaseFee = GetBaseFee(vehicle.VehicleType);
            vehicle.AdditionalFee = GetFee(vehicle.VehicleType);
            vehicle.HelmetsFee = GetHelmetsFee();
            return vehicle;
        }

        public static VehicleRecord AddCheckOutInfoForVehicle(VehicleRecord vehicle)
        {
            vehicle.CheckOut = DateTime.Now.ToLocalTime();
            vehicle.Done = true;

            var baseFee = GetBaseFee(vehicle.VehicleType);
            var additionalHours = CalculateAdditionalHours(vehicle);
            var additionalFee = additionalHours * GetFee(vehicle.VehicleType);
            var helmetsFee = CalculateHelmetsFee(vehicle);

            var totalFee = baseFee + additionalFee + helmetsFee;
            totalFee = totalFee > 0 ? totalFee : 0;

            vehicle.BaseFee = baseFee;
            vehicle.AdditionalHours = additionalHours;
            vehicle.AdditionalFee = additionalFee;
            vehicle.HelmetsFee = helmetsFee;
            vehicle.Fee = totalFee;

            return vehicle;
        }

        private static int CalculateAdditionalHours(VehicleRecord vehicle)
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

        private static double CalculateHelmetsFee(VehicleRecord vehicle)
        {
            return GetHelmetsFee() * vehicle.Helmets;
        }
    }
}
