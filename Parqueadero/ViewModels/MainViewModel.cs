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

        private SettingsViewModel _settings;
        public SettingsViewModel Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
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
            ShowSettingsCommand = new Command(ShowSettings, () => !Busy);

            SyncVehicles();
        }

        public Command DoCheckInCommand { get; }
        private async void DoCheckIn()
        {
            Busy = true;
            SyncVehicles();
            ReleaseResources();
            CheckIn = new CheckInViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new CheckInPage());
            Busy = false;
        }

        public Command DoCheckOutCommand { get; }
        private async void DoCheckOut()
        {
            Busy = true;
            SyncVehicles();
            ReleaseResources();
            CheckOut = new CheckOutViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new CheckOutPage());
            Busy = false;
        }

        public Command ShowSummaryCommand { get; }
        private async void ShowSummary()
        {
            Busy = true;
            SyncVehicles();
            ReleaseResources();
            Summary = new SummaryViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new SummaryPage());
            Busy = false;
        }

        public Command ShowSettingsCommand { get; }
        private async void ShowSettings()
        {
            Busy = true;
            ReleaseResources();
            Settings = new SettingsViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            Busy = false;
        }

        public async Task SyncVehicles()
        {
            if (!loadingVehicles && !String.IsNullOrWhiteSpace(Settings.ParkingLotId))
            {
                loadingVehicles = true;
                await ((DataService)Application.Current.Resources["DataService"]).SyncAsync();
                loadingVehicles = false;
            }
        }

        public void ReleaseResources()
        {
            CheckIn = null;
            CheckOut = null;
            Summary = null;
            Settings = null;
        }
    }
}
