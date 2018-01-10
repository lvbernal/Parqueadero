using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Parqueadero.Services;
using Parqueadero.Models;

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

                _currentVehicle = ParkingLot.Instance.AddCheckOutInfoForVehicle(_currentVehicle);

                VehicleType = _currentVehicle.VehicleType;
                Plate = _currentVehicle.Plate;
                CheckInTime = _currentVehicle.CheckIn;
                CheckOutTime = _currentVehicle.CheckOut;
                Helmets = _currentVehicle.Helmets;
                TotalFee = _currentVehicle.Fee;
                BaseFee = _currentVehicle.BaseFee;
                HelmetsFee = _currentVehicle.HelmetsFee;
                AdditionalHours = _currentVehicle.AdditionalHours;
                AdditionalFee = _currentVehicle.AdditionalFee;

                DoScan = false;
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
            }
        }

        private string _noReceiptPlate;
        public string NoReceiptPlate
        {
            get { return _noReceiptPlate; }
            set
            {
                _noReceiptPlate = value;
                NotifyPropertyChanged();
            }
        }

        private double _totalFee;
        public double TotalFee
        {
            get { return _totalFee; }
            set
            {
                _totalFee = value;
                NotifyPropertyChanged();
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
                CheckOutCommand.ChangeCanExecute();
            }
        }

        public CheckOutViewModel()
        {
            CheckOutCommand = new Command(CheckOut, () => !SavingAndPrinting);
            CurrentVehicle = null;
            DoScan = true;
        }

        public Command CheckOutCommand { get; }
        public async void CheckOut()
        {
            SavingAndPrinting = true;

            await ((DataService)Application.Current.Resources["DataService"]).SaveVehicle(CurrentVehicle);
            var printed = await Print();

            if (!printed)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No fue posible imprimir el recibo, pero la salida fue registrada.", "OK");
            }

            await Application.Current.MainPage.Navigation.PopAsync();

            SavingAndPrinting = false;
        }

        private async Task<bool> Print()
        {
            return await ((PrintService)Application.Current.Resources["PrintService"]).Print(CurrentVehicle);
        }

        public async Task<bool> LoadVehicle(string code)
        {
            var vehicle = await ((DataService)Application.Current.Resources["DataService"]).GetVehicle(code);

            if (vehicle != null)
            {
                CurrentVehicle = vehicle;
                return true;
            }

            return false;
        }
    }
}
