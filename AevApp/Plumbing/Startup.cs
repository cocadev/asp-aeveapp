using AevApp.Plumbing.IoC;
using AevApp.Plumbing.Mapper;
using AevApp.View;
using AevApp.ViewModel;
using Autofac;
using AutoMapper;
using CommonServiceLocator;
using Xamarin.SDK.Plumbing;
using Xamarin.SDK.Service.Interface;

namespace AevApp.Plumbing
{
    public class Startup : StartupBase
    {
        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterModule<CrossPlatforModule>();
            builder.RegisterModule<MvvmModule>();

            builder.RegisterType<MapperProfile>().As<Profile>();
            builder.RegisterModule<MapperModule>();
        }

        public override void MapViewsToViewModels()
        {
            var pageService = ServiceLocator.Current.GetInstance<IPageService>();
            pageService.Map(typeof(HomePage), typeof(HomeViewModel));
            pageService.Map(typeof(OfficerDashboardPage), typeof(OfficerDashboardViewModel));
            pageService.Map(typeof(SecurityCheckSubmissionPage), typeof(SecurityCheckSubmissionViewModel));
            pageService.Map(typeof(SecurityCheckSubmissionPage_Portrait), typeof(SecurityCheckSubmissionViewModel));
            pageService.Map(typeof(SetupPage), typeof(SetupViewModel));
            pageService.Map(typeof(AddVpPage), typeof(AddVpViewModel));
            pageService.Map(typeof(RestartAgainPage), typeof(RestartAgainViewModel));
        }
    }
}