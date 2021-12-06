using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace AevApp.Model
{
    public class Location : ObservableObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;

                _isSelected = value;
                UpdateBackgroundColor();
                RaisePropertyChanged(() => IsSelected);
            }
        }

        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor == value) return;
                _backgroundColor = value;
                RaisePropertyChanged(() => BackgroundColor);
            }
        }

        private void UpdateBackgroundColor()
        {
            BackgroundColor = this.IsSelected ? (Color)Application.Current.Resources["AevBlueGrey"] : Color.Transparent;
        }
    }
}