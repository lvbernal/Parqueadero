using System;
namespace Parqueadero.ViewModels
{
    public class SummaryItemViewModel : BindableBase
    {
        private string _image;
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                NotifyPropertyChanged();
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyPropertyChanged();
            }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }
    }
}
