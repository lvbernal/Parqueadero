using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Parqueadero.Models;
using Parqueadero.Helpers;
using Parqueadero.Services;

namespace Parqueadero.ViewModels
{
    public class CheckInViewModel : BindableBase
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

        private ObservableCollection<VehicleOptionViewModel> VehicleOptions { get; set; }

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
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

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
                if (SelectedVehicle == null) { return ""; }

                var helmetsRate = Parking.GetHelmetsFee() * Helmets;
                var baseFee = Parking.GetBaseFee(SelectedVehicle.VehicleType);
                var fee = Parking.GetFee(SelectedVehicle.VehicleType);

                if (helmetsRate > 0)
                {
                    var format = "${0} + {1}/h+, + ${2}";
                    return String.Format(format, baseFee, fee, helmetsRate);
                }
                else
                {
                    var format = "${0} + {1}/h+";
                    return String.Format(format, baseFee, fee);
                }
            }
        }

        private bool IsValid
        {
            get
            {
                return Plate.Length > 0 && SelectedVehicle != null;
            }
        }

        private bool _savingAndPrinting;
        public bool SavingAndPrinting
        {
            get { return _savingAndPrinting; }
            set
            {
                _savingAndPrinting = value;
                NotifyPropertyChanged();
                CheckInCommand.ChangeCanExecute();
            }
        }

        public CheckInViewModel()
        {
            AddHelmetCommand = new Command(AddHelmet);
            RemoveHelmetCommand = new Command(RemoveHelmet);
            CheckInCommand = new Command(CheckIn, () => !SavingAndPrinting);
            InitializeVehicleOptions();
        }

        private void InitializeVehicleOptions()
        {
            CarOption = new VehicleOptionViewModel() { VehicleType = Constants.Car };
            PickupOption = new VehicleOptionViewModel() { VehicleType = Constants.Pickup };
            TruckOption = new VehicleOptionViewModel() { VehicleType = Constants.Truck };
            MotorbikeOption = new VehicleOptionViewModel() { VehicleType = Constants.Motorbike };
            BikeOption = new VehicleOptionViewModel() { VehicleType = Constants.Bike };

            CarOption.PropertyChanged += (s, e) => ValidateSelection(CarOption);
            PickupOption.PropertyChanged += (s, e) => ValidateSelection(PickupOption);
            TruckOption.PropertyChanged += (s, e) => ValidateSelection(TruckOption);
            MotorbikeOption.PropertyChanged += (s, e) => ValidateSelection(MotorbikeOption);
            BikeOption.PropertyChanged += (s, e) => ValidateSelection(BikeOption);

            VehicleOptions = new ObservableCollection<VehicleOptionViewModel>()
            {
                CarOption, PickupOption, TruckOption, MotorbikeOption, BikeOption
            };
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
            SavingAndPrinting = true;

            if (IsValid)
            {
                var vehicle = BuildVehicle();
                var printed = await Print(vehicle);

                if (printed)
                {
                    await Save(vehicle);
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alerta", "No fue posible imprimir el recibo. El ingreso no fue registrado.", "OK");
                }
            }

            SavingAndPrinting = false;
        }

        private VehicleRecord BuildVehicle()
        {
            VehicleRecord vehicle = new VehicleRecord()
            {
                ParkingLotId = Parking.Id,
                Plate = Plate,
                VehicleType = SelectedVehicle.VehicleType,
                CheckIn = DateTime.Now.ToLocalTime(),
                Helmets = Helmets
            };

            return vehicle;
        }

        private async Task<bool> Print(VehicleRecord vehicle)
        {
            var printService = new PrintService(Parking.PrinterUrl);
            return await printService.PrintCheckIn(vehicle);
        }

        private async Task Save(VehicleRecord vehicle)
        {
            var dataService = (DataService)Application.Current.Resources["DataService"];
            await dataService.SaveVehicle(vehicle);
        }
    }
}
