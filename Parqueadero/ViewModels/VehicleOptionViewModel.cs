using System;
using Xamarin.Forms;

namespace Parqueadero.ViewModels
{
    public class VehicleOptionViewModel : BindableBase
    {
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

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
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

        private double _fee;
        public double Fee
        {
            get { return _fee; }
            set
            {
                _fee = value;
                NotifyPropertyChanged();
            }
        }

        public Command ChangeSelectCommand { get; }
        public void ChangeSelect()
        {
            if (!Selected)
            {
                Selected = true;
            }
        }

        public VehicleOptionViewModel()
        {
            ChangeSelectCommand = new Command(ChangeSelect);
        }
    }
}
