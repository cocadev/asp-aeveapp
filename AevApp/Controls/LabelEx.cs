using Xamarin.Forms;

namespace AevApp.Controls
{
    public class LabelEx : Label
    {
        public static readonly BindableProperty ObservableTextProperty =
            BindableProperty.Create("ObservableText", typeof(string), typeof(LabelEx), string.Empty, BindingMode.TwoWay);

        public string ObservableText
        {
            get => (string)GetValue(ObservableTextProperty);
            set
            {
                SetValue(ObservableTextProperty, value);
            }
        }
    }
}