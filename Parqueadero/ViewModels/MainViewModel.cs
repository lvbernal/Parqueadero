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

        public MainViewModel()
        {
            DoCheckInCommand = new Command(DoCheckIn);
            DoCheckOutCommand = new Command(DoCheckOut);

            Parking = new ParkingLot();
        }

        public Command DoCheckInCommand { get; }
        public async void DoCheckIn()
        {
            CheckIn = new CheckInViewModel() { Parking = Parking };
            await Application.Current.MainPage.Navigation.PushAsync(new CheckInPage());
        }

        public Command DoCheckOutCommand { get; }
        public async void DoCheckOut()
        {
            CheckOut = new CheckOutViewModel() { Parking = Parking };
            await Application.Current.MainPage.Navigation.PushAsync(new CheckOutPage());
        }

        public async Task ReloadVehicles()
        {
            var dataService = (DataService)Application.Current.Resources["DataService"];
            await dataService.GetVehiclesAsync();
        }
    }
}
