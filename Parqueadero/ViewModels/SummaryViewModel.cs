using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Parqueadero.Models;
using Parqueadero.Services;

namespace Parqueadero.ViewModels
{
    public class SummaryViewModel : BindableBase
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

        public DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SummaryItem> _items = new ObservableCollection<SummaryItem>();
        public ObservableCollection<SummaryItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<VehicleRecord> _vehicles = new ObservableCollection<VehicleRecord>();
        public ObservableCollection<VehicleRecord> Vehicles
        {
            get { return _vehicles; }
            set
            {
                _vehicles = value;
                NotifyPropertyChanged();
            }
        }

        public SummaryViewModel()
        {
            Date = DateTime.Now.ToLocalTime();
        }

        public async void LoadSummary()
        {
            await LoadValues();
            await LoadVehicles();
        }

        private async Task LoadValues()
        {
            Items.Add(new SummaryItem() { Image = "cash.png", Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vcar.png", Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vpickup.png", Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vtruck.png", Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vmotorbike.png", Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vbike.png", Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "equal.png", Value = "Pendiente" });
        }

        private async Task LoadVehicles()
        {
            var vehicles = await Data.GetVehiclesAsync();

            if (vehicles != null)
            {
                foreach (var vehicle in vehicles)
                {
                    Vehicles.Add(vehicle);
                }
            }
        }
    }
}
