using Xamarin.Forms;

namespace AevApp.Controls
{
    public class StartAevButton : BugFreeImageButton
    {
        private readonly string _defaultState = "startAEV.png";
        private readonly string _pressedState = "startAEVPressed.png";
        public StartAevButton()
        {
            //ImageButton bug: background cannot be transparent
            //Otherwise on Press the image will reduce in size
            //Currently set in custom renderer in BugFreeImageButton
            WidthRequest = 240;
            HeightRequest = 107;
            Source = _defaultState;

            VisualStateManager.SetVisualStateGroups(this, new VisualStateGroupList()
            {
                new VisualStateGroup()
                {
                    Name = "CommonNormal",
                    States =
                    {
                        new VisualState{ Name = "Normal", Setters = { new Setter()
                        {
                            Property = SourceProperty,
                            Value = _defaultState
                        }}},
                        new VisualState{ Name = "Pressed", Setters = { new Setter()
                        {
                            Property = SourceProperty,
                            Value = _pressedState
                        }}}
                    },
                },

            });
        }
    }
}