using Xamarin.Forms;

namespace AevApp.Controls
{
    public class EntryEx : Entry
    {
        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create("BorderColor", typeof(Color), typeof(FormEntry), Color.CornflowerBlue);

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }
    }
}