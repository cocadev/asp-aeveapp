using System;
using Xamarin.Forms;

namespace AevApp.Controls
{
    public class AnimatedButton : ContentView
    {
        private LabelEx _textLabel;
        
        private StackLayout _layout;

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(AnimatedButton), string.Empty);

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create("FontFamily", typeof(string), typeof(AnimatedButton), string.Empty);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create("TextColor", typeof(Color), typeof(AnimatedButton), Color.White);
        
        public static readonly BindableProperty TabCommandProperty =
            BindableProperty.Create("TabCommand", typeof(Command), typeof(AnimatedButton), default(Command));


        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public Command TabCommand
        {
            get => (Command)GetValue(TabCommandProperty);
            set => SetValue(TabCommandProperty, value);
        }


        /// <summary>
        /// Creates a new instance of the animation button
        /// </summary>
        public AnimatedButton()
        {
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Orientation = StackOrientation.Horizontal,
                Padding = 5,
                BackgroundColor = BackgroundColor
            };

            // create the label
            _textLabel = new LabelEx
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                Text = Text,
                TextColor = TextColor,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Start,
            };
            _layout.Children.Add(_textLabel);

            // add a gester reco
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async (o) =>
                {
                    await this.ScaleTo(0.95, 50, Easing.CubicOut);
                    await this.ScaleTo(1, 50, Easing.CubicIn);
                    TabCommand.Execute(null);
                })
            });

            // set the content
            this.Content = _layout;
        }
    }
}