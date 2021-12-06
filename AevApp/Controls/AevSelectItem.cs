using System.Linq;
using AevApp.Model.NavArgs;
using Xamarin.Forms;

namespace AevApp.Controls
{
    public class AevSelectItem : ContentView
    {
        private readonly Frame _frame;
        private readonly LabelEx _label;

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create("IsSelected", typeof(bool), typeof(AevSelectItem), false, BindingMode.TwoWay);

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(AevSelectItem), string.Empty, BindingMode.TwoWay);

        private StackLayout _stack;


        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Thickness RowMargin { get; set; }

        public AevSelectItem()
        {
            _label = new LabelEx
            {
                TextColor = (Color) Application.Current.Resources["AevDarkBlue"],
                FontSize = (double) Application.Current.Resources["MediumText"],
                Text = "Place Holder",
                ObservableText = "Place Holder",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };

            _stack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(15, 0, 0, 0)
            };
            _stack.Children.Add(_label);

            _frame = new Frame
            {
                BorderColor = (Color)Application.Current.Resources["AevBlueGrey"],
                CornerRadius = (int)Application.Current.Resources["SmallBorderRadius"],
                Content = _stack,
                Margin = SetMargin(),
                Padding = new Thickness(0),
                HasShadow = false,
                BackgroundColor = Color.Transparent
            };
            UpdateBackgroundColor();
            Content = _frame;
        }

        public void OnIsSelectedChanged()
        {
            UpdateBackgroundColor();
        }

        private void UpdateBackgroundColor()
        {
            _stack.BackgroundColor = IsSelected ? (Color) Application.Current.Resources["AevBlueGrey"] : Color.Transparent;
            _label.TextColor = IsSelected ? Color.White : (Color) Application.Current.Resources["AevDarkBlue"];
        }

        public void OnTextChanged()
        {
            _label.ObservableText = Text;
            _label.Text = Text;
        }

        public void OnBackgroundColorChanged()
        {
//            _stack.BackgroundColor = BackgroundColor;
        }

        public void UpdateMargin()
        {
            _frame.Margin = SetMargin();
        }

        private Thickness SetMargin()
        {
            return RowMargin == default(Thickness) ? new Thickness(50, 0, 50, 10) : RowMargin;
        }
    }
}