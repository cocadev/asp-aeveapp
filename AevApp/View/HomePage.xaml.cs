using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms.Xaml;

namespace AevApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : PageBase
    {
        public HomePage()
        {
            InitializeComponent();
            Title = "AEV Mobile";
        }

    }
}