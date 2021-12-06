using AevApp.Helper;
using AevApp.Helper.Implementation;
using AevApp.Helper.Interface;
using AevApp.Service.Implementation;
using AevApp.Service.Interface;
using Autofac;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Face;
using Xamarin.SDK.Helper.Implementation;
using Xamarin.SDK.Helper.Interface;
using Xamarin.SDK.PageBase.Implementation;
using Xamarin.SDK.PageBase.Interface;
using Xamarin.SDK.Service.Implementation;
using Xamarin.SDK.Service.Interface;

namespace AevApp.Plumbing.IoC
{
    public class CrossPlatforModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterHelpers(builder);

            RegisterServices(builder);

            RegisterMapper(builder);
        }

        private void RegisterMapper(ContainerBuilder builder)
        {
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<Messenger>()
                .As<IMessenger>()
                .SingleInstance();
            
            builder.RegisterType<NavigationService>()
                .As<INavigationService>()
                .SingleInstance();

            builder.RegisterType<PageService>()
                .As<IPageService>()
                .SingleInstance();

            builder.RegisterType<NavigationPageBase>()
                .As<INavigationPage>()
                .SingleInstance();
            
            builder.RegisterType<AppDialogService>()
                .As<IAppDialogService>()
                .SingleInstance();
            
            builder.RegisterType<ApplicationStateService>()
                .As<IApplicationStateService>()
                .SingleInstance();

            builder.RegisterType<ConfigManager>()
                .As<IConfigManager>()
                .SingleInstance();

            builder.RegisterType<ApiClientService>()
                .As<IApiClientService>()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterType<AuthManager>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<AppSettings>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<FaceService>()
                .As<IFaceService>()
                .SingleInstance();

            builder.RegisterType<StorageService>()
                .As<IStorageService>()
                .SingleInstance();

            builder.RegisterType<GeneralAppSettings>()
                .As<IAppSettings>()
                .SingleInstance();
        }

        private void RegisterHelpers(ContainerBuilder builder)
        {
            builder.RegisterType<TypeResolver>()
                .As<ITypeResolver>()
                .SingleInstance();

            builder.RegisterType<DeviceHelper>()
                .As<IDeviceHelper>()
                .SingleInstance();
        }
    }
}