using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Xamarin.Forms;

namespace AevApp.Controls
{
    public class AevOverlay : ContentView
    {
        private readonly Color _purpleTextColor = Color.FromHex("#705C9B");
        private readonly int _buttonCornerRadius = 15;
        private AbsoluteLayout _windowLayout;
        private BoxView _backgroundView;
        private StackLayout _contentLayout;
        private ActivityIndicator _activityIndicator;
        private LabelEx _loadingMessageLabel;
        private Grid _dialogGrid;
        private LabelEx _dialogMessageLabel;
        private ButtonEx _cancelButton;
        private ButtonEx _confirmButton;

        public static readonly BindableProperty IsActivityIndicatorVisibleProperty =
            BindableProperty.Create("IsActivityIndicatorVisible", typeof(bool), typeof(AevOverlay), false, BindingMode.TwoWay);

        public static readonly BindableProperty LoadingMessageProperty =
            BindableProperty.Create("LoadingMessage", typeof(string), typeof(AevOverlay), string.Empty, BindingMode.TwoWay);

        public static readonly BindableProperty DialogMessageProperty =
            BindableProperty.Create("DialogMessage", typeof(string), typeof(AevOverlay), string.Empty, BindingMode.TwoWay);

        public static readonly BindableProperty ConfirmBtnTextProperty =
            BindableProperty.Create("ConfirmBtnText", typeof(string), typeof(AevOverlay), "YES", BindingMode.TwoWay);

        public static readonly BindableProperty CancelBtnTextProperty =
            BindableProperty.Create("CancelBtnText", typeof(string), typeof(AevOverlay), "NO", BindingMode.TwoWay);

        public static readonly BindableProperty IsDialogVisibleProperty =
            BindableProperty.Create("IsDialogVisible", typeof(bool), typeof(AevOverlay), false, BindingMode.TwoWay);

        public static readonly BindableProperty ConfirmCommandProperty =
            BindableProperty.Create("ConfirmCommand", typeof(ICommand), typeof(AevOverlay), null, BindingMode.TwoWay);

        public static readonly BindableProperty CancelCommandProperty =
            BindableProperty.Create("CancelCommand", typeof(ICommand), typeof(AevOverlay), null, BindingMode.TwoWay);

        public bool IsActivityIndicatorVisible
        {
            get => (bool)GetValue(IsActivityIndicatorVisibleProperty);
            set => SetValue(IsActivityIndicatorVisibleProperty, value);
        }

        public string LoadingMessage
        {
            get => (string)GetValue(LoadingMessageProperty);
            set => SetValue(LoadingMessageProperty, value);
        }

        public string DialogMessage
        {
            get => (string)GetValue(DialogMessageProperty);
            set => SetValue(DialogMessageProperty, value);
        }

        public string ConfirmBtnText
        {
            get => (string)GetValue(ConfirmBtnTextProperty);
            set => SetValue(ConfirmBtnTextProperty, value);
        }

        public string CancelBtnText
        {
            get => (string)GetValue(CancelBtnTextProperty);
            set => SetValue(CancelBtnTextProperty, value);
        }

        public bool IsDialogVisible
        {
            get => (bool)GetValue(IsDialogVisibleProperty);
            set => SetValue(IsDialogVisibleProperty, value);
        }

        public ICommand ConfirmCommand
        {
            get => (ICommand)GetValue(ConfirmCommandProperty);
            set => SetValue(ConfirmCommandProperty, value);
        }

        public ICommand CancelCommand
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        public AevOverlay()
        {
            InitWindowLayout();
            InitBackgroundOverlay();
            InitContentLayout();
            InitActivityIndicator();
            InitLoadingMessage();
            InitDialog();

            Content = _windowLayout;
        }

        private void InitDialog()
        {
            _dialogGrid = new Grid
            {
                BackgroundColor = Color.White,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition {Height = 20},
                    new RowDefinition {Height = new GridLength(2, GridUnitType.Star)},
                    new RowDefinition {Height = 30},
                    new RowDefinition {Height = GridLength.Star},
                    new RowDefinition {Height = 40},
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition {Width = 40},
                    new ColumnDefinition {Width = GridLength.Star},
                    new ColumnDefinition {Width = 50},
                    new ColumnDefinition {Width = GridLength.Star},
                    new ColumnDefinition {Width = 40},
                },
                IsVisible = IsDialogVisible
            };
            _dialogMessageLabel = new LabelEx
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                ObservableText = DialogMessage,
                Text = DialogMessage,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = _purpleTextColor
            };
            Grid.SetColumnSpan(_dialogMessageLabel, 3);
            Grid.SetColumn(_dialogMessageLabel, 1);
            Grid.SetRow(_dialogMessageLabel, 1);
            _dialogGrid.Children.Add(_dialogMessageLabel);

            _cancelButton = new ButtonEx
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(ButtonEx)),
                Text = CancelBtnText,
                Command = CancelCommand,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.End,
                CornerRadius = _buttonCornerRadius,
                TextColor = Color.FromHex("#C81B4F"),
                BorderColor = Color.FromHex("#FF7BA3"),
                BorderWidth = 1
            };
            _dialogGrid.Children.Add(_cancelButton, 1, 3);

            _confirmButton = new ButtonEx
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(ButtonEx)),
                Text = ConfirmBtnText,
                Command = ConfirmCommand,
                HorizontalOptions = LayoutOptions.Start,
                CornerRadius = _buttonCornerRadius,
                BorderColor = Color.FromHex("#4F8AB6"),
                TextColor = Color.FromHex("#4F8AB6"),
                BackgroundColor = Color.Transparent,
                BorderWidth = 1
            };
            Grid.SetColumn(_confirmButton, 3);
            Grid.SetRow(_confirmButton, 3);
            _dialogGrid.Children.Add(_confirmButton);

            _contentLayout.Children.Add(_dialogGrid);
        }

        private void InitLoadingMessage()
        {
            _loadingMessageLabel = new LabelEx
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Margin = new Thickness(0, 10, 0, 0),
                ObservableText = LoadingMessage,
                Text = LoadingMessage,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Transparent
            };
            _contentLayout.Children.Add(_loadingMessageLabel);
        }

        private void InitActivityIndicator()
        {
            _activityIndicator = new ActivityIndicator
            {
                IsRunning = true,
                Color = Color.FromHex("#60EBB9"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                IsVisible = IsActivityIndicatorVisible
            };
            _contentLayout.Children.Add(_activityIndicator);
        }

        private void InitContentLayout()
        {
            var contentView = new ContentView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };
            _contentLayout = new StackLayout
            {
                BackgroundColor = Color.Transparent
            };
            AbsoluteLayout.SetLayoutFlags(contentView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(contentView, new Rectangle(0, 0, 1, 1));
            contentView.Content = _contentLayout;

            _windowLayout.Children.Add(contentView);
        }

        private void InitWindowLayout()
        {
            _windowLayout = new AbsoluteLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
        }

        private void InitBackgroundOverlay()
        {
            _backgroundView = new BoxView
            {
                BackgroundColor = BackgroundColor == Color.Default ? (Color)Application.Current.Resources["AevPurple"] : BackgroundColor,
                Opacity = 0.9
            };
            AbsoluteLayout.SetLayoutFlags(_backgroundView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(_backgroundView, new Rectangle(0, 0, 1, 1));

            _windowLayout.Children.Add(_backgroundView);
        }

        public void UpdateLoadingMessage()
        {
            _loadingMessageLabel.ObservableText = LoadingMessage;
        }

        public void UpdateActivityIndicatorVisibility()
        {
            _activityIndicator.IsVisible = IsActivityIndicatorVisible;
        }

        public void UpdateDialogVisibility()
        {
            _dialogGrid.IsVisible = IsDialogVisible;
        }

        public void UpdateDialogMessage()
        {
            _dialogMessageLabel.ObservableText = DialogMessage;
        }

        public void UpdateCancelBtnText()
        {
            _cancelButton.Text = CancelBtnText;
            _cancelButton.IsVisible = !string.IsNullOrWhiteSpace(CancelBtnText);
        }

        public void UpdateConfirmBtnText()
        {
            _confirmButton.Text = ConfirmBtnText;
            _confirmButton.IsVisible = !string.IsNullOrWhiteSpace(ConfirmBtnText);
        }

        private void CloseDialog()
        {
            IsVisible = false;
            IsDialogVisible = false;
            _windowLayout.IsVisible = false;
            _contentLayout.IsVisible = false;
            _dialogGrid.IsVisible = false;
        }

        public void UpdateConfirmCommand()
        {
            _confirmButton.Command = ConfirmCommand;
        }

        public void UpdateCancelCommand()
        {
            _cancelButton.Command = CancelCommand;
        }
    }
}