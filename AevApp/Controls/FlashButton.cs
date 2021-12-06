using System.Collections.Generic;
using AevApp.Model.Enum;
using Xamarin.Forms;

namespace AevApp.Controls
{
    public class FlashButton : BugFreeImageButton
    {
        public static readonly BindableProperty CurrentStateProperty =
            BindableProperty.Create("CurrentState", typeof(FlashState), typeof(FlashButton), FlashState.Off, BindingMode.TwoWay);

        public FlashState CurrentState
        {
            get => (FlashState)GetValue(CurrentStateProperty);
            set => SetValue(CurrentStateProperty, value);
        }

        private readonly Dictionary<FlashState, string> _buttonState = new Dictionary<FlashState, string>
        {
            {FlashState.Off, "flashOFF.png" },
            {FlashState.On, "flashON.png" },
            {FlashState.Auto, "flashAUTO.png" }
        };

        public FlashButton()
        {
            WidthRequest = 40;
            HeightRequest = 40;
            Source = _buttonState[FlashState.Off];
        }

        public void UpdateState()
        {
            Source = _buttonState[CurrentState];
        }
    }
}