using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Parqueadero.Models;
using Parqueadero.Services;
using Xamarin.Forms;

namespace Parqueadero.ViewModels
{
    public class SummaryViewModel : BindableBase
    {
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

        private SummaryItemViewModel _carSummary = new SummaryItemViewModel() { Image = "vcar.png", Text = "Carros", Value = "0" };
        public SummaryItemViewModel CarSummary
        {
            get { return _carSummary; }
            set
            {
                _carSummary = value;
                NotifyPropertyChanged();
            }
        }

        private SummaryItemViewModel _pickupSummary = new SummaryItemViewModel() { Image = "vpickup.png", Text = "Camionetas", Value = "0" };
        public SummaryItemViewModel PickupSummary
        {
            get { return _pickupSummary; }
            set
            {
                _pickupSummary = value;
                NotifyPropertyChanged();
            }
        }

        private SummaryItemViewModel _truckSummary = new SummaryItemViewModel() { Image = "vtruck.png", Text = "Camiones", Value = "0" };
        public SummaryItemViewModel TruckSummary
        {
            get { return _truckSummary; }
            set
            {
                _truckSummary = value;
                NotifyPropertyChanged();
            }
        }

        private SummaryItemViewModel _motorbikeSummary = new SummaryItemViewModel() { Image = "vmotorbike.png", Text = "Motos", Value = "0" };
        public SummaryItemViewModel MotorbikeSummary
        {
            get { return _motorbikeSummary; }
            set
            {
                _motorbikeSummary = value;
                NotifyPropertyChanged();
            }
        }

        private SummaryItemViewModel _bikeSummary = new SummaryItemViewModel() { Image = "vbike.png", Text = "Bicicletas", Value = "0" };
        public SummaryItemViewModel BikeSummary
        {
            get { return _bikeSummary; }
            set
            {
                _bikeSummary = value;
                NotifyPropertyChanged();
            }
        }

        public SummaryViewModel()
        {
            Date = DateTime.Now.ToLocalTime();
        }

        private async Task LoadValues()
        {
            Vehicles.Clear();
            await LoadCurrentVehicles();
        }

        private async Task LoadCurrentVehicles()
        {
            var vehicles = await ((DataService)Application.Current.Resources["DataService"]).GetVehiclesAsync();
            var countDict = new Dictionary<string, int>
            {
                { "car", 0 },
                { "pickup", 0 },
                { "truck", 0 },
                { "motorbike", 0 },
                { "bike", 0 }
            };

            if (vehicles != null)
            {
                foreach (var vehicle in vehicles)
                {
                    countDict[vehicle.VehicleType]++;
                    Vehicles.Add(vehicle);
                }

                CarSummary.Value = countDict["car"].ToString();
                PickupSummary.Value = countDict["pickup"].ToString();
                TruckSummary.Value = countDict["truck"].ToString();
                MotorbikeSummary.Value = countDict["motorbike"].ToString();
                BikeSummary.Value = countDict["bike"].ToString();
            }
        }

        /*
        private async Task LoadSummaryByDate()
        {
            var dateStr = Date.Date.ToString("yyyy-MM-dd");
            Items.Add(new SummaryItem() { Image = "cash.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vcar.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vpickup.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vtruck.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vmotorbike.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "vbike.png", Text = dateStr, Value = "Pendiente" });
            Items.Add(new SummaryItem() { Image = "equal.png", Text = dateStr, Value = "Pendiente" });
        }
        */

    }
}
