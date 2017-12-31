using System;
using System.Collections.Generic;
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
                LoadValues();
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

        private Dictionary<string, int> _countDict;
        public Dictionary<string, int> CountDict
        {
            get { return _countDict; }
            set
            {
                _countDict = value;
                NotifyPropertyChanged();
            }
        }

        public SummaryViewModel()
        {
            CountDict = new Dictionary<string, int>
            {
                { "car", 0 },
                { "pickup", 0 },
                { "truck", 0 },
                { "motorbike", 0 },
                { "bike", 0 }
            };
        }

        public async void LoadSummary()
        {
			await LoadVehicles();
            Date = DateTime.Now.ToLocalTime();
        }

        private async Task LoadValues()
        {
            Items.Clear();

            var dateStr = Date.Date.ToString("yyyy-MM-dd");

            Items.Add(new SummaryItem() { Image = "cash.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vcar.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vpickup.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vtruck.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vmotorbike.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vbike.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "equal.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vcar.png", Text = "Actual", Value = CountDict["car"].ToString() });
            Items.Add(new SummaryItem() { Image = "vpickup.png", Text = "Actual", Value = CountDict["pickup"].ToString() });
            Items.Add(new SummaryItem() { Image = "vtruck.png", Text = "Actual", Value = CountDict["truck"].ToString() });
            Items.Add(new SummaryItem() { Image = "vmotorbike.png", Text = "Actual", Value = CountDict["motorbike"].ToString() });
            Items.Add(new SummaryItem() { Image = "vbike.png", Text = "Actual", Value = CountDict["bike"].ToString() });
        }

        private async Task LoadVehicles()
        {
            var vehicles = await Data.GetVehiclesAsync();

            if (vehicles != null)
            {
                foreach (var vehicle in vehicles)
                {
                    CountDict[vehicle.VehicleType]++;
                    Vehicles.Add(vehicle);
                }
            }
        }
    }
}
