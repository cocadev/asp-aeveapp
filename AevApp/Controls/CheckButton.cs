using Xamarin.Forms;

namespace AevApp.Controls
{
    public class CheckButton : BugFreeImageButton
    {
        private string _defaultState = "CheckButtonDark.png";
        private string _pressedState = "CheckButtonDarkPressed.png";

        public string DefaultStateImageSource
        {
            get => _defaultState;
            set
            {
                _defaultState = value;
                this.Source = _defaultState;
            }
        }

        public string PressedStateImageSource
        {
            get => _pressedState;
            set => _pressedState = value;
        }

        public CheckButton()
        {
            //ImageButton bug: background cannot be transparent
            //Otherwise on Press the image will reduce in size
            //Currently set in custom renderer in BugFreeImageButton
            WidthRequest = 53.33;
            HeightRequest = 53.33;
            Source = _defaultState;

            this.Pressed += (sender, args) =>
            {
                this.Source = string.IsNullOrWhiteSpace(PressedStateImageSource)
                    ? _pressedState
                    : PressedStateImageSource;
            };

            this.Released += (sender, args) =>
            {
                this.Source = string.IsNullOrWhiteSpace(DefaultStateImageSource)
                    ? _defaultState
                    : DefaultStateImageSource;
            };
        }
    }
}