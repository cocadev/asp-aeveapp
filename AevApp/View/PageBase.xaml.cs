using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.SDK.PageBase.Interface;
using Xamarin.SDK.ViewModel;

namespace AevApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageBase : ContentPage, ICleanUpPage
	{
		public PageBase ()
		{
			InitializeComponent ();
		}

	    public async Task CleanUp()
	    {
	        var vm = this.BindingContext as AppViewModelBase;
	        await vm?.OnPopped();
	        vm?.Cleanup();
        }
	}
}