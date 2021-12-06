using System;
using Xamarin.Forms;

namespace AevApp
{
	public partial class MainPage : ContentPage
	{
	    public static event EventHandler<ImageSource> PhotoCapturedEvent;

        public MainPage()
		{
			InitializeComponent();
		}

	    public static void OnPhotoCaptured(ImageSource src)
	    {
	        PhotoCapturedEvent?.Invoke(new MainPage(), src);
	    }
    }
}
