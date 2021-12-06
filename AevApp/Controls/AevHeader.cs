using System;
using System.Windows.Input;
using AevApp.Model;
using Xamarin.Forms;

namespace AevApp.Controls
{
    public class AevHeader : ContentView
    {
        private Grid _layout;
        private Image _logoImage;
        private LabelEx _usernameLabel;
        private LabelEx _roleLabel;


        public static readonly BindableProperty UsernameProperty =
            BindableProperty.Create("Username", typeof(string), typeof(AevHeader), string.Empty);

        public static readonly BindableProperty RoleProperty =
            BindableProperty.Create("Role", typeof(string), typeof(AevHeader), string.Empty);

        public static readonly BindableProperty TabCommandProperty =
            BindableProperty.Create("TabCommand", typeof(ICommand), typeof(AevHeader), null);

        public static readonly BindableProperty CancelCommandProperty =
            BindableProperty.Create("CancelCommand", typeof(ICommand), typeof(AevHeader), null);

        private readonly Color _textColor = (Color)Application.Current.Resources["AevDarkBlue"];

        public bool ShowCancel { get; set; } = false;

        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public string Role
        {
            get => (string)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }

        public ICommand TabCommand
        {
            get => (ICommand)GetValue(TabCommandProperty);
            set => SetValue(TabCommandProperty, value);
        }

        public ICommand CancelCommand
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        public AevHeader()
        {
            _layout = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = (Color)Application.Current.Resources["AevLightGrey"],
                HeightRequest = 75
            };

            if (Global.Orientation == Constants.Orientation.Landscape)
            {
                _layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Absolute) });
                _layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
                _layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            else
            {
                _layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
                _layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
                _layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.5, GridUnitType.Star) });
                _layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.5, GridUnitType.Star) });
            }

            //            AddLogoImage();

            AddLogoText();
            Content = _layout;
        }

        private void AddLogoText()
        {
            var label = new LabelEx
            {
                Text = "Melbourne Airport",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(10, 0, 0, 0),
                TextColor = _textColor,
                FontSize = (double)Application.Current.Resources["LargeText"],
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.Transparent
            };
            _layout.Children.Add(label, 0, 0);
        }

        private void AddLogoImage()
        {
            _logoImage = new Image
            {
                Source = "logo.png",
            };
            _layout.Children.Add(_logoImage, 1, 0);
        }

        public void UpdateUsername()
        {
            _usernameLabel.Text = Username;
        }

        public void UpdateRole()
        {
            _roleLabel.Text = Role;
        }

        public void ShowUserInfo()
        {
            var userinfoGrid = new Grid();

            userinfoGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 15 });
            userinfoGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            userinfoGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 80 });
            userinfoGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 10 });

            var stackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };

            _usernameLabel = new LabelEx
            {
                Text = Username,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                TextColor = _textColor,
                FontSize = (double)Application.Current.Resources["SmallText"],
                FontFamily = (string)Application.Current.Resources["OpenSansBold"]
            };
            stackLayout.Children.Add(_usernameLabel);

            _roleLabel = new LabelEx
            {
                Text = Role,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                TextColor = _textColor,
                FontSize = (double)Application.Current.Resources["SmallText"],
                FontFamily = (string)Application.Current.Resources["OpenSansBold"]
            };
            stackLayout.Children.Add(_roleLabel);
            _layout.Children.Add(stackLayout, 1, 0);

            //Logout button
            var logoutBtn = new AevImageButton
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = 40,
                Command = TabCommand,
                DefaultStateImageSource = "Logout.png",
                PressedStateImageSource = "LogoutPressed.png",
            };
            _layout.Children.Add(logoutBtn, 2, 0);

            //cancel button
            if (ShowCancel)
            {
                var cancelBtn = new AevImageButton
                {
                    VerticalOptions = LayoutOptions.Center,
                    HeightRequest = 35,
                    Command = CancelCommand,
                    DefaultStateImageSource = "closeButton.png",
                    PressedStateImageSource = "closeButtonPressed.png"
                };
                _layout.Children.Add(cancelBtn, 3, 0);
            }

            //Cancel button
            //            var cancelBtn = new BugFreeImageButton
            //            {
            //                Source = ""
            //            }
        }
    }
}