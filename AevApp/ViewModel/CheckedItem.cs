using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Xamarin.Forms;

namespace AevApp.ViewModel
{
    public class CheckedItem : ObservableObject
    {
        public CheckedItem()
        {
            ToggleCheckedCommand = new RelayCommand<CheckedItem>(ToggleChecked);
        }

        private void ToggleChecked(CheckedItem checkedItem)
        {
            checkedItem.Checked = !checkedItem.Checked;
        }

        private string _id;

        public string Id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        private bool _checked;

        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                UpdateBackgroundColor();
                RaisePropertyChanged(() => Checked);
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
            BackgroundColor = this.Checked ? (Color)Application.Current.Resources["AevBlueGrey"] : Color.Transparent;
        }
        public ICommand ToggleCheckedCommand { get; set; }
    }
}