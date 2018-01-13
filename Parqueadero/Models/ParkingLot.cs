using System;
using Parqueadero.Helpers;

namespace Parqueadero.Models
{
    public class ParkingLot
    {
        private static int hourToleranceInMinutes = 10;
        private static double carBase = 2500;
        private static double carFee = 2500;
        private static double pickupBase = 2500;
        private static double pickupFee = 2500;
        private static double truckBase = 3000;
        private static double truckFee = 3000;
        private static double motorbikeBase = 1500;
        private static double motorbikeFee = 500;
        private static double bikeBase = 1000;
        private static double bikeFee = 0;
        private static double helmetsBase = 500;

        public static double GetBaseFee(string vehicleType)
        {
            switch (vehicleType)
            {
                case Constants.Car:
                    return carBase;
                case Constants.Pickup:
                    return pickupBase;
                case Constants.Truck:
                    return truckBase;
                case Constants.Motorbike:
                    return motorbikeBase;
                case Constants.Bike:
                    return bikeBase;
                default:
                    return 0;
            }
        }

        public static double GetFee(string vehicleType)
        {
            switch (vehicleType)
            {
                case Constants.Car:
                    return carFee;
                case Constants.Pickup:
                    return pickupFee;
                case Constants.Truck:
                    return truckFee;
                case Constants.Motorbike:
                    return motorbikeFee;
                case Constants.Bike:
                    return bikeFee;
                default:
                    return 0;
            }
        }

        public static double GetHelmetsFee()
        {
            return helmetsBase;
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

            if (hours > 0 && minutes <= hourToleranceInMinutes)
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
