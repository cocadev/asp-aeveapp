using Xamarin.Forms;

namespace AevApp.Controls
{
    public class AevImageButton : BugFreeImageButton
    {
        public static readonly BindableProperty DefaultStateImageSourceProperty =
            BindableProperty.Create("DefaultStateImageSource", typeof(string), typeof(AevImageButton), string.Empty);

        public static readonly BindableProperty PressedStateImageSourceProperty =
            BindableProperty.Create("PressedStateImageSource", typeof(string), typeof(AevImageButton), string.Empty);

        public string DefaultStateImageSource
        {
            get => (string)GetValue(DefaultStateImageSourceProperty);
            set => SetValue(DefaultStateImageSourceProperty, value);
        }

        public string PressedStateImageSource
        {
            get => (string)GetValue(PressedStateImageSourceProperty);
            set => SetValue(PressedStateImageSourceProperty, value);
        }

        public AevImageButton()
        {
            this.Pressed += (sender, args) => { UpdateImageSource(PressedStateImageSource); };

            this.Released += (sender, args) => { UpdateImageSource(DefaultStateImageSource); };
        }

        public void UpdateImageSource(string imageSource)
        {
            Source = imageSource;
        }
    }
}