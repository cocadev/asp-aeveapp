using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AevApp.Controls
{
    public class FormEntry : ContentView
    {
        #region Private Fields
        private LabelEx _titleLabel;
        private EntryEx _valueEntry;
        private DatePicker _datePicker;
        private LabelEx _errorMsgLabel;
        private StackLayout _layout;
        #endregion

        #region Bindable properties

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create("Title", typeof(string), typeof(FormEntry), string.Empty, BindingMode.Default);

        public static readonly BindableProperty HasTitleProperty =
            BindableProperty.Create("HasTitle", typeof(bool), typeof(FormEntry), true, BindingMode.Default);

        public static readonly BindableProperty TextProperty =
           BindableProperty.Create("Text", typeof(string), typeof(FormEntry), string.Empty);

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create("FontFamily", typeof(string), typeof(FormEntry), string.Empty);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create("TextColor", typeof(Color), typeof(FormEntry), (Color)Application.Current.Resources["AevLightGrey"]);

        public static readonly BindableProperty IsPasswordProperty =
            BindableProperty.Create("IsPassword", typeof(bool), typeof(FormEntry), false);

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create("BorderColor", typeof(Color), typeof(FormEntry), (Color)Application.Current.Resources["AevLightGrey"]);

        public static readonly BindableProperty ErrorMessageProperty =
            BindableProperty.Create("ErrorMessage", typeof(string), typeof(FormEntry), string.Empty);

        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create("IsValid", typeof(bool), typeof(FormEntry), true);

        public static readonly BindableProperty RequiredProperty =
            BindableProperty.Create("Required", typeof(bool), typeof(FormEntry), false);

        public static readonly BindableProperty IsDateTimeEntryProperty =
            BindableProperty.Create("IsDateTimeEntry", typeof(bool), typeof(FormEntry), false);

        public static readonly BindableProperty ValidationTriggerProperty =
            BindableProperty.Create("ValidationTrigger", typeof(int), typeof(FormEntry), 0);

        private string _datePickerFormat = "yyyy-MM-dd";

        #endregion

        #region Public Properties
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public bool HasTitle
        {
            get => (bool)GetValue(HasTitleProperty);
            set => SetValue(HasTitleProperty, value);
        }

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

        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set
            {
                if (_valueEntry != null)
                {
                    _valueEntry.IsPassword = value;
                }
                SetValue(IsPasswordProperty, value);
            }
        }

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }
        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set
            {
                SetValue(IsValidProperty, value);
                _errorMsgLabel.IsVisible = !value;
            }
        }

        public bool Required
        {
            get => (bool)GetValue(RequiredProperty);
            set => SetValue(RequiredProperty, value);
        }

        public bool IsDateTimeEntry
        {
            get => (bool)GetValue(IsDateTimeEntryProperty);
            set => SetValue(IsDateTimeEntryProperty, value);
        }

        public bool ValidationTrigger
        {
            get => (bool)GetValue(ValidationTriggerProperty);
            set => SetValue(ValidationTriggerProperty, value);
        }
        #endregion

        /// <summary>
        /// Initialise UI in Constructor
        /// </summary>
        public FormEntry()
        {
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Orientation = StackOrientation.Vertical,
                //                Padding = 5,
                BackgroundColor = Color.Transparent
            };

            AddTitle();
            AddEntry();
            AddErrorMsg();

            Content = _layout;
        }

        private void AddErrorMsg()
        {
            _errorMsgLabel = new LabelEx()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontFamily = FontFamily,
                Text = ErrorMessage,
                TextColor = Color.OrangeRed,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Start,
                IsVisible = false,
                BackgroundColor = Color.Transparent
            };

            _layout.Children.Add(_errorMsgLabel);
        }

        private void AddEntry()
        {
            if (!HasTitle) return;

            if (IsDateTimeEntry)
            {
                InitDatePicker();
                _layout.Children.Add(_datePicker);
            }
            else
            {
                _valueEntry = new EntryEx()
                {
                    Text = Text,
                    IsPassword = IsPassword,
                    TextColor = (Color)Application.Current.Resources["AevDarkBlue"],
                    FontFamily = FontFamily,
                    BorderColor = BorderColor
                };
                _valueEntry.TextChanged += UpdateText;
                _layout.Children.Add(_valueEntry);
            }
        }

        private void InitDatePicker()
        {
            _datePicker = new DatePicker
            {
                TextColor = TextColor,
                FontFamily = FontFamily,
                Date = string.IsNullOrWhiteSpace(Text) ? DateTime.Now : DateTime.ParseExact(Text, _datePickerFormat, null),
                Format = _datePickerFormat
            };

            _datePicker.DateSelected += (sender, args) => { Text = args.NewDate.ToString(_datePickerFormat); };
        }

        private void AddTitle()
        {
            var textSize = (double)Application.Current.Resources["MediumText"];
            _titleLabel = new LabelEx
            {
                FontSize = textSize,
                FontFamily = FontFamily,
                Text = Title,
                TextColor = TextColor,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Start,
                BackgroundColor = Color.Transparent
            };

            _layout.Children.Add(_titleLabel);
        }

        public void UpdateText(object sender, TextChangedEventArgs e)
        {
            Text = e.NewTextValue;
        }

        public void UpdateText()
        {
            this._valueEntry.Text = Text;
        }

        public void UpdateTitle()
        {
            if (!HasTitle)
            {
                _layout.Children.RemoveAt(0);
            }
            else
            {
                this._titleLabel.Text = Title;
            }
        }

        public void UpdateErrorMsg()
        {
            this._errorMsgLabel.Text = ErrorMessage;
        }

        public void UpdateIsPassword()
        {
            this._valueEntry.IsPassword = IsPassword;
        }

        public void ValidateFormEntry()
        {
            //Not Ideal. Assuming when Required, we only validate the text not being empty
            if (Required)
            {
                IsValid = !string.IsNullOrWhiteSpace(Text);
            }

            _errorMsgLabel.IsVisible = !IsValid && !string.IsNullOrWhiteSpace(ErrorMessage);

            //Trigger border color changes
            _valueEntry.BorderColor = IsValid ? Color.Default : Color.FromHex("#C75050");
        }

        public void CheckDateTimeControl()
        {
            if (IsDateTimeEntry)
            {
                this._layout.Children.RemoveAt(HasTitle ? 1 : 0);
                InitDatePicker();
                _layout.Children.Insert(1, _datePicker);
            }
        }
    }
}