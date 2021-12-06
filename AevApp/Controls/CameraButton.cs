using System;
using System.Collections.Generic;
using AevApp.Model.Enum;
using Xamarin.Forms;

namespace AevApp.Controls
{
    public class CameraButton : BugFreeImageButton
    {
        public static readonly BindableProperty CurrentStateProperty =
            BindableProperty.Create("CurrentState", typeof(CameraButtonState), typeof(CameraButton), CameraButtonState.Default, BindingMode.TwoWay);

        public CameraButtonState CurrentState
        {
            get => (CameraButtonState)GetValue(CurrentStateProperty);
            set => SetValue(CurrentStateProperty, value);
        }

        private readonly Dictionary<CameraButtonState, string> _buttonState = new Dictionary<CameraButtonState, string>
        {
            {CameraButtonState.Default, "cameraButton.png" },
            {CameraButtonState.Pressed, "cameraButtonPressed.png" },
            {CameraButtonState.Taking, "cameraButtonRed.png" },
            {CameraButtonState.Complete, "cameraButtonGreen.png" }
        };

        public CameraButton()
        {
            WidthRequest = 66.67;
            HeightRequest = 66.67;
            Source = _buttonState[CameraButtonState.Default];

            Pressed += (sender, args) =>
            {
                Source = _buttonState[CameraButtonState.Pressed];
            };

            Released += (sender, args) =>
            {
                Source = _buttonState[CameraButtonState.Default];
            };
        }

        public void UpdateState()
        {
            Source = _buttonState[CurrentState];
        }
    }
}