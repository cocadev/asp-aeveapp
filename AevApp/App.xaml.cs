using System;
using AevApp.Helper.Interface;
using AevApp.Service.Interface;
using AevApp.View;
using AevApp.ViewModel;
using Autofac;
using CommonServiceLocator;
using Xamarin.Forms;
using Xamarin.SDK.PageBase.Implementation;
using Xamarin.SDK.Plumbing;

namespace AevApp
{
	public partial class App : Application
	{
	    private IApplicationStateService _applicationStateService;
        
        public App() : this(null) { }

        public App (IStartup startup)
		{
			InitializeComponent();
		    try
		    {
		        var mainNavPage = new HomePage();
		        var navigationPage = new NavigationPageBase(mainNavPage);

		        var builder = new ContainerBuilder();
		        startup.Init(builder, navigationPage);
		        startup.MapViewsToViewModels();
                var vm = ServiceLocator.Current.GetInstance<HomeViewModel>();
		        mainNavPage.BindingContext = vm;

		        MainPage = navigationPage;

		    }
		    catch (Exception ex)
		    {

		    }
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		    if (_applicationStateService == null)
		    {
		        _applicationStateService = ServiceLocator.Current.GetInstance<IApplicationStateService>();
		    }
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
