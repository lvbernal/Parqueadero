using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Parqueadero.Services;
using Parqueadero.Models;
using Parqueadero.Helpers;

namespace Parqueadero.ViewModels
{
    public class CheckOutViewModel : BindableBase
    {
        private VehicleRecord _currentVehicle;
        public VehicleRecord CurrentVehicle
        {
            get { return _currentVehicle; }
            set
            {
                if (value != null)
                {
                    _currentVehicle = value;
                    _currentVehicle.CheckOut = DateTime.Now.ToLocalTime();
                    NotifyPropertyChanged();

                    DoScan = false;

                    VehicleType = _currentVehicle.VehicleType;
                    Plate = CurrentVehicle.Plate;
                    CheckInTime = CurrentVehicle.CheckIn;
                    CheckOutTime = CurrentVehicle.CheckOut;
                    Helmets = CurrentVehicle.Helmets;

                    BaseFee = Constants.GetBaseFee(CurrentVehicle.VehicleType);
                    HelmetsFee = Constants.GetHelmetsFee() * Helmets;

                    var difference = CheckOutTime - CheckInTime;
                    var hours = difference.Hours;
                    var minutes = difference.Minutes;
                    hours -= minutes > Constants.HourToleranceInMinutes ? 0 : 1;
                    AdditionalFee = hours * Constants.GetFee(CurrentVehicle.VehicleType);
                }
            }
        }

        private bool _doScan;
        public bool DoScan
        {
            get { return _doScan; }
            set
            {
                _doScan = value;
                NotifyPropertyChanged();
            }
        }

        private string _vehicleType;
        public string VehicleType
        {
            get { return _vehicleType; }
            set
            {
                _vehicleType = value;
                NotifyPropertyChanged();
            }
        }

        private string _plate;
        public string Plate
        {
            get { return _plate; }
            set
            {
                _plate = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime _checkInTime;
        public DateTime CheckInTime
        {
            get { return _checkInTime; }
            set
            {
                _checkInTime = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime _checkOutTime;
        public DateTime CheckOutTime
        {
            get { return _checkOutTime; }
            set
            {
                _checkOutTime = value;
                NotifyPropertyChanged();
            }
        }

        private int _helmets;
        public int Helmets
        {
            get { return _helmets; }
            set
            {
                _helmets = value;
                NotifyPropertyChanged();
            }
        }

        private double _baseFee;
        public double BaseFee
        {
            get { return _baseFee; }
            set
            {
                _baseFee = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TotalFee));
            }
        }

        private double _additionalFee;
        public double AdditionalFee
        {
            get { return _additionalFee; }
            set
            {
                _additionalFee = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TotalFee));
            }
        }

        private double _helmetsFee;
        public double HelmetsFee
        {
            get { return _helmetsFee; }
            set
            {
                _helmetsFee = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TotalFee));
            }
        }

        public double TotalFee
        {
            get
            {
                return BaseFee + AdditionalFee + HelmetsFee;
            }
        }

        public CheckOutViewModel()
        {
            CheckOutCommand = new Command(CheckOut);
            CurrentVehicle = null;
            DoScan = true;
        }

        public Command CheckOutCommand { get; }
        public async void CheckOut()
        {
            CurrentVehicle.Fee = TotalFee;
            CurrentVehicle.Done = true;

            var dataService = (DataService)Application.Current.Resources["DataService"];
            await dataService.SaveVehicle(CurrentVehicle);
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public async Task<bool> LoadVehicle(string code)
        {
            var dataService = (DataService)Application.Current.Resources["DataService"];
            var vehicle = await dataService.GetVehicle(code);

            if (vehicle != null)
            {
                CurrentVehicle = vehicle;
                return true;
            }

            return false;
        }
    }
}
