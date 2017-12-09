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
                _currentVehicle = value;
                NotifyPropertyChanged();

                if (_currentVehicle != null)
                {
                    _currentVehicle.CheckOut = DateTime.Now.ToLocalTime();

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

                    if (hours > 0 && minutes <= Constants.HourToleranceInMinutes)
                    {
                        hours--;
                    }

                    AdditionalHours = hours;
                    AdditionalFee = hours * Constants.GetFee(CurrentVehicle.VehicleType);

                    DoScan = false;
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

        private int _additionalHours;
        public int AdditionalHours
        {
            get { return _additionalHours; }
            set
            {
                _additionalHours = value;
                NotifyPropertyChanged();
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

        private string _noReceiptPlate = "";
        public string NoReceiptPlate
        {
            get { return _noReceiptPlate; }
            set
            {
                _noReceiptPlate = value;
                NotifyPropertyChanged();
            }
        }

        public double TotalFee
        {
            get
            {
                return BaseFee + AdditionalFee + HelmetsFee;
            }
        }

        private bool _printing;
        public bool Printing
        {
            get { return _printing; }
            set
            {
                _printing = value;
                NotifyPropertyChanged();
                CheckOutCommand.ChangeCanExecute();
            }
        }

        public CheckOutViewModel()
        {
            CheckOutCommand = new Command(CheckOut, () => !Printing);
            CurrentVehicle = null;
            DoScan = true;
        }

        public Command CheckOutCommand { get; }
        public async void CheckOut()
        {
            Printing = true;

            CurrentVehicle.Fee = TotalFee;
            CurrentVehicle.Done = true;

            var dataService = (DataService)Application.Current.Resources["DataService"];
            await dataService.SaveVehicle(CurrentVehicle);

            var printService = (PrintService)Application.Current.Resources["PrintService"];
            var printed = await printService.PrintCheckOut(CurrentVehicle);

            if (!printed)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No fue posible imprimir el recibo.", "OK");
            }

            await Application.Current.MainPage.Navigation.PopAsync();

            NoReceiptPlate = "";
            Printing = false;
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
