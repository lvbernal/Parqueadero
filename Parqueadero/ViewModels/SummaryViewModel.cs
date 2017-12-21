using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Parqueadero.Models;
using Parqueadero.Services;

namespace Parqueadero.ViewModels
{
    public class SummaryViewModel : BindableBase
    {
        private ParkingLot _parking;
        public ParkingLot Parking
        {
            get { return _parking; }
            set
            {
                _parking = value;
                NotifyPropertyChanged();
            }
        }

        public DataService Data { get; set; }

        public DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged();
            }
        }

        private double _fee;
        public double Fee
        {
            get { return _fee; }
            set
            {
                _fee = value;
                NotifyPropertyChanged();
            }
        }

        private int _car;
        public int Car
        {
            get { return _car; }
            set
            {
                _car = value;
                NotifyPropertyChanged();
            }
        }

        private int _pickup;
        public int Pickup
        {
            get { return _pickup; }
            set
            {
                _pickup = value;
                NotifyPropertyChanged();
            }
        }

        private int _truck;
        public int Truck
        {
            get { return _truck; }
            set
            {
                _truck = value;
                NotifyPropertyChanged();
            }
        }

        private int _motorbike;
        public int Motorbike
        {
            get { return _motorbike; }
            set
            {
                _motorbike = value;
                NotifyPropertyChanged();
            }
        }

        private int _bike;
        public int Bike
        {
            get { return _bike; }
            set
            {
                _bike = value;
                NotifyPropertyChanged();
            }
        }

        private int _total;
        public int Total
        {
            get { return _total; }
            set
            {
                _total = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<VehicleRecord> _vehicles = new ObservableCollection<VehicleRecord>();
        public ObservableCollection<VehicleRecord> Vehicles
        {
            get { return _vehicles; }
            set
            {
                _vehicles = value;
                NotifyPropertyChanged();
            }
        }

        public SummaryViewModel()
        {
            Date = DateTime.Now.ToLocalTime();
        }

        public async void LoadVehicles()
        {
            var vehicles = await Data.GetVehiclesAsync();

            if (vehicles != null)
            {
                foreach (var vehicle in vehicles)
                {
                    Vehicles.Add(vehicle);
                }
            }
        }
    }
}
