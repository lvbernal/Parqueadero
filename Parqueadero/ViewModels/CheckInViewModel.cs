using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Parqueadero.Models;
using Parqueadero.Helpers;
using Parqueadero.Services;

namespace Parqueadero.ViewModels
{
    public class CheckInViewModel : BindableBase
    {
        private int _helmets = 0;
        public int Helmets
        {
            get { return _helmets; }
            set
            {
                _helmets = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Fee));
            }
        }

        private string _plate = "";
        public string Plate
        {
            get { return _plate; }
            set
            {
                _plate = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public string Fee
        {
            get
            {
                var helmetsRate = Constants.HelmetsBase * Helmets;

                if (helmetsRate > 0)
                {
                    var format = "${0} + {1}/h+, + ${2}";
                    return String.Format(format, SelectedVehicle.BaseFee, SelectedVehicle.Fee, helmetsRate);
                }
                else
                {
                    var format = "${0} + {1}/h+";
                    return String.Format(format, SelectedVehicle.BaseFee, SelectedVehicle.Fee);
                }
            }
        }

        private bool IsValid
        {
            get
            {
                return Plate.Length > 0;
            }
        }

        private VehicleOptionViewModel _carOption;
        public VehicleOptionViewModel CarOption
        {
            get { return _carOption; }
            set
            {
                _carOption = value;
                NotifyPropertyChanged();
            }
        }

        private VehicleOptionViewModel _pickupOption;
        public VehicleOptionViewModel PickupOption
        {
            get { return _pickupOption; }
            set
            {
                _pickupOption = value;
                NotifyPropertyChanged();
            }
        }

        private VehicleOptionViewModel _truckOption;
        public VehicleOptionViewModel TruckOption
        {
            get { return _truckOption; }
            set
            {
                _truckOption = value;
                NotifyPropertyChanged();
            }
        }

        private VehicleOptionViewModel _motorbikeOption;
        public VehicleOptionViewModel MotorbikeOption
        {
            get { return _motorbikeOption; }
            set
            {
                _motorbikeOption = value;
                NotifyPropertyChanged();
            }
        }

        private VehicleOptionViewModel _bikeOption;
        public VehicleOptionViewModel BikeOption
        {
            get { return _bikeOption; }
            set
            {
                _bikeOption = value;
                NotifyPropertyChanged();
            }
        }

        private VehicleOptionViewModel _selectedVehicle;
        public VehicleOptionViewModel SelectedVehicle
        {
            get { return _selectedVehicle; }
            set
            {
                _selectedVehicle = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Fee));
            }
        }

        private ObservableCollection<VehicleOptionViewModel> _vehicleOptions;
        public ObservableCollection<VehicleOptionViewModel> VehicleOptions
        {
            get { return _vehicleOptions; }
            set
            {
                _vehicleOptions = value;
                NotifyPropertyChanged();
            }
        }

        public CheckInViewModel()
        {
            AddHelmetCommand = new Command(AddHelmet);
            RemoveHelmetCommand = new Command(RemoveHelmet);
            CheckInCommand = new Command(CheckIn);

            CarOption = new VehicleOptionViewModel() { VehicleType = VehicleRecord.Car, BaseFee = Constants.CarBase, Fee = Constants.CarFee };
            PickupOption = new VehicleOptionViewModel() { VehicleType = VehicleRecord.Pickup, BaseFee = Constants.PickupBase, Fee = Constants.PickupFee };
            TruckOption = new VehicleOptionViewModel() { VehicleType = VehicleRecord.Truck, BaseFee = Constants.TruckBase, Fee = Constants.TruckFee };
            MotorbikeOption = new VehicleOptionViewModel() { VehicleType = VehicleRecord.Motorbike, BaseFee = Constants.MotorbikeBase, Fee = Constants.MotorbikeFee };
            BikeOption = new VehicleOptionViewModel() { VehicleType = VehicleRecord.Bike, BaseFee = Constants.BikeBase, Fee = Constants.BikeFee };

            CarOption.PropertyChanged += (s, e) => ValidateSelection(CarOption);
            PickupOption.PropertyChanged += (s, e) => ValidateSelection(PickupOption);
            TruckOption.PropertyChanged += (s, e) => ValidateSelection(TruckOption);
            MotorbikeOption.PropertyChanged += (s, e) => ValidateSelection(MotorbikeOption);
            BikeOption.PropertyChanged += (s, e) => ValidateSelection(BikeOption);

            VehicleOptions = new ObservableCollection<VehicleOptionViewModel>()
            {
                CarOption, PickupOption, TruckOption, MotorbikeOption, BikeOption
            };

            CarOption.Selected = true;
        }

        private void ValidateSelection(VehicleOptionViewModel vehicle)
        {
            if (vehicle.Selected)
            {
                SelectedVehicle = vehicle;

                foreach (var otherVehicle in VehicleOptions)
                {
                    if (otherVehicle != SelectedVehicle)
                    {
                        otherVehicle.Selected = false;
                    }
                }
            }
        }

        public Command AddHelmetCommand { get; }
        public void AddHelmet()
        {
            Helmets++;
        }

        public Command RemoveHelmetCommand { get; }
        public void RemoveHelmet()
        {
            if (Helmets > 0) { Helmets--; }
        }

        public Command CheckInCommand { get; }
        public async void CheckIn()
        {
            if (!IsValid) { return; }

            VehicleRecord vehicle = new VehicleRecord()
            {
                ParkingLotId = Constants.ParkingLotId,
                Plate = Plate,
                VehicleType = SelectedVehicle.VehicleType,
                CheckIn = DateTime.Now.ToLocalTime(),
                Helmets = Helmets
            };

            var dataService = (DataService)Application.Current.Resources["DataService"];
            await dataService.SaveVehicle(vehicle);
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
