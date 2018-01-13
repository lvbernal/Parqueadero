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
                var helmetsRate = ParkingLot.GetHelmetsFee() * Helmets;
                var baseFee = ParkingLot.GetBaseFee(SelectedVehicle?.VehicleType);
                var fee = ParkingLot.GetFee(SelectedVehicle?.VehicleType);

                if (helmetsRate > 0)
                {
                    return String.Format("${0} + {1}/h+, + ${2}", baseFee, fee, helmetsRate);
                }
                else
                {
                    return String.Format("${0} + {1}/h+", baseFee, fee);
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

            var vehicleOptions = new ObservableCollection<VehicleOptionViewModel>()
            {
                CarOption, PickupOption, TruckOption, MotorbikeOption, BikeOption
            };

            ConfigureUniqueSelection(vehicleOptions);
        }

        private void ConfigureUniqueSelection(ObservableCollection<VehicleOptionViewModel> vehicleOptions)
        {
            foreach (var vehicle in vehicleOptions)
            {
                vehicle.PropertyChanged += delegate
                {
                    if (vehicle.Selected)
                    {
                        SelectedVehicle = vehicle;

                        foreach (var otherVehicle in vehicleOptions)
                        {
                            if (otherVehicle != SelectedVehicle)
                            {
                                otherVehicle.Selected = false;
                            }
                        }
                    }
                };
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

            if (await CanCreateVehicle())
            {
                var vehicle = BuildVehicle();
                var printed = await Print(vehicle);

                if (printed)
                {
                    await ((DataService)Application.Current.Resources["DataService"]).SaveVehicle(vehicle);
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alerta", "No fue posible imprimir el recibo. El ingreso no fue registrado.", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", "El vehículo ya está registrado.", "OK");
            }

            SavingAndPrinting = false;
        }

        private async Task<bool> CanCreateVehicle()
        {
            var existing = await ((DataService)Application.Current.Resources["DataService"]).GetVehicle(Plate);
            return existing == null;
        }

        private VehicleRecord BuildVehicle()
        {
            VehicleRecord vehicle = new VehicleRecord()
            {
                Plate = Plate,
                VehicleType = SelectedVehicle.VehicleType,
                Helmets = Helmets
            };

            return ParkingLot.AddCheckInInfoForVehicle(vehicle);
        }

        private async Task<bool> Print(VehicleRecord vehicle)
        {
            return await ((PrintService)Application.Current.Resources["PrintService"]).Print(vehicle);
        }
    }
}
