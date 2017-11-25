using System;
using Parqueadero.Models;

namespace Parqueadero.Helpers
{
    public class Constants
    {
        public static string ApplicationURL = (string)Environment.GetEnvironmentVariable("BACKEND_URL_PARQ");
        public static string ParkingLotId = (string)Environment.GetEnvironmentVariable("PARKINGLOT_ID");

        public static double CarBase = 2500;
        public static double CarFee = 2500;
        public static double PickupBase = 2500;
        public static double PickupFee = 2500;
        public static double TruckBase = 3000;
        public static double TruckFee = 3000;
        public static double MotorbikeBase = 1500;
        public static double MotorbikeFee = 500;
        public static double BikeBase = 1000;
        public static double BikeFee = 0;

        public static double HelmetsBase = 500;

        public static int HourToleranceInMinutes = 10;

        public static string PrintCheckInError = "No fue posible imprimir el recibo.";
        public static string PrintCheckOutError = "No fue posible imprimir el recibo.";


        public static double GetBaseFee(string vehicleType)
        {
            switch (vehicleType)
            {
                case VehicleRecord.Car:
                    return CarBase;
                case VehicleRecord.Pickup:
                    return PickupBase;
                case VehicleRecord.Truck:
                    return TruckBase;
                case VehicleRecord.Motorbike:
                    return MotorbikeBase;
                case VehicleRecord.Bike:
                    return BikeBase;
            }

            return 0;
        }

        public static double GetFee(string vehicleType)
        {
            switch (vehicleType)
            {
                case VehicleRecord.Car:
                    return CarFee;
                case VehicleRecord.Pickup:
                    return PickupFee;
                case VehicleRecord.Truck:
                    return TruckFee;
                case VehicleRecord.Motorbike:
                    return MotorbikeFee;
                case VehicleRecord.Bike:
                    return BikeFee;
            }

            return 0;
        }

        public static double GetHelmetsFee()
        {
            return HelmetsBase;
        }
    }
}
