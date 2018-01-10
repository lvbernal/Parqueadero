using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Parqueadero.Pages;
using Parqueadero.Services;

namespace Parqueadero.ViewModels
{
    public class MainViewModel : BindableBase
    {
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

        private SummaryViewModel _summary;
        public SummaryViewModel Summary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                NotifyPropertyChanged();
            }
        }

        private bool _busy;
        public bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                NotifyPropertyChanged();
                DoCheckInCommand.ChangeCanExecute();
                DoCheckOutCommand.ChangeCanExecute();
                ShowSummaryCommand.ChangeCanExecute();
            }
        }

        private bool loadingVehicles = false;

        public MainViewModel()
        {
            DoCheckInCommand = new Command(DoCheckIn, () => !Busy);
            DoCheckOutCommand = new Command(DoCheckOut, () => !Busy);
            ShowSummaryCommand = new Command(ShowSummary, () => !Busy);

            SyncVehicles();
        }

        public Command DoCheckInCommand { get; }
        public async void DoCheckIn()
        {
            Busy = true;
            SyncVehicles();
            CheckIn = new CheckInViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new CheckInPage());
            Busy = false;
        }

        public Command DoCheckOutCommand { get; }
        public async void DoCheckOut()
        {
            Busy = true;
            SyncVehicles();
            CheckOut = new CheckOutViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new CheckOutPage());
            Busy = false;
        }

        public Command ShowSummaryCommand { get; }
        public async void ShowSummary()
        {
            Busy = true;
            SyncVehicles();
            Summary = new SummaryViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new SummaryPage());
            Busy = false;
        }

        public async Task SyncVehicles()
        {
            if (!loadingVehicles)
            {
                loadingVehicles = true;
                await ((DataService)Application.Current.Resources["DataService"]).SyncAsync();
                loadingVehicles = false;
            }
        }
    }
}
