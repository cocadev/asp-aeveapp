using System.Windows.Input;
using Xamarin.Forms;

namespace AevApp.Controls
{
    public class AevDropdown : ContentView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(AevDropdown), string.Empty, BindingMode.TwoWay);

        public static readonly BindableProperty TabCommandProperty =
            BindableProperty.Create("TabCommand", typeof(ICommand), typeof(AevDropdown), null);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand TabCommand
        {
            get => (ICommand)GetValue(TabCommandProperty);
            set => SetValue(TabCommandProperty, value);
        }

        private AbsoluteLayout _dropdownLayout;
        private BackgroundBoxView _containerBoxView;
        private LabelEx _textLabel;

        public AevDropdown()
        {
            _dropdownLayout = new AbsoluteLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent
            };

            var outerFrame = new Frame
            {
                BackgroundColor = (Color)Application.Current.Resources["AevLightGrey"],
                CornerRadius = (int)Application.Current.Resources["SmallBorderRadius"],
                Padding = new Thickness(2),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HasShadow = false
            };
            _containerBoxView = new BackgroundBoxView
            {
                StartColor = Color.White,
                EndColor = (Color)Application.Current.Resources["AevLightGrey"],
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = (int)Application.Current.Resources["SmallBorderRadius"],
            };

            _textLabel = new LabelEx
            {
                Text = Text,
                ObservableText = Text,
                BackgroundColor = Color.Transparent,
                TextColor = (Color)Application.Current.Resources["AevDarkBlue"],
                FontSize = (double)Application.Current.Resources["MediumText"],
                FontFamily = (string)Application.Current.Resources["OpenSansBold"],
                LineBreakMode = LineBreakMode.TailTruncation
            };

            var caretDownLabel = new LabelEx
            {
                Text = "\uf0d7",
                BackgroundColor = Color.Transparent,
                TextColor = (Color)Application.Current.Resources["AevDarkBlue"],
                FontSize = (double)Application.Current.Resources["MediumText"],
                FontFamily = "fonts/FontAwesome"
            };
            outerFrame.Content = _containerBoxView;
            _dropdownLayout.Children.Add(outerFrame, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _dropdownLayout.Children.Add(_textLabel, new Rectangle(6, 10, 0.85, 32), AbsoluteLayoutFlags.WidthProportional);
            _dropdownLayout.Children.Add(caretDownLabel, new Rectangle(1, 14, 20, 32), AbsoluteLayoutFlags.XProportional);

            Content = _dropdownLayout;
        }

        public void UpdateText()
        {
            _textLabel.ObservableText = Text;
        }

        public void UpdateCommand()
        {
            _dropdownLayout.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = TabCommand
            });
        }
    }
}