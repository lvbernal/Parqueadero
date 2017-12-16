using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Parqueadero.Pages;
using Parqueadero.Services;
using Parqueadero.Models;

namespace Parqueadero.ViewModels
{
    public class MainViewModel : BindableBase
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

        private CheckInViewModel _checkIn;
        public CheckInViewModel CheckIn
        {
            get { return _checkIn; }
            set
            {
                _checkIn = value;
                NotifyPropertyChanged();
            }
        }

        private CheckOutViewModel _checkOut;
        public CheckOutViewModel CheckOut
        {
            get { return _checkOut; }
            set
            {
                _checkOut = value;
                NotifyPropertyChanged();
            }
        }

        private bool loadingVehicles = false;

        public MainViewModel()
        {
            DoCheckInCommand = new Command(DoCheckIn);
            DoCheckOutCommand = new Command(DoCheckOut);
            ShowSummaryCommand = new Command(ShowSummary);

            Parking = new ParkingLot();
            Data = new DataService(Parking);

            SyncVehicles();
        }

        public Command DoCheckInCommand { get; }
        public async void DoCheckIn()
        {
            SyncVehicles();
            CheckIn = new CheckInViewModel() { Parking = Parking, Data = Data };
            await Application.Current.MainPage.Navigation.PushAsync(new CheckInPage());
        }

        public Command DoCheckOutCommand { get; }
        public async void DoCheckOut()
        {
            SyncVehicles();
            CheckOut = new CheckOutViewModel() { Parking = Parking, Data = Data };
            await Application.Current.MainPage.Navigation.PushAsync(new CheckOutPage());
        }

        public Command ShowSummaryCommand { get; }
        public async void ShowSummary()
        {
            SyncVehicles();
            await Application.Current.MainPage.Navigation.PushAsync(new SummaryPage());
        }

        public async Task SyncVehicles()
        {
            if (!loadingVehicles)
            {
                loadingVehicles = true;
                await Data.SyncAsync();
                loadingVehicles = false;
            }
        }
    }
}
