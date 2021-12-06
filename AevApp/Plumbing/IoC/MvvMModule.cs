using AevApp.View;
using AevApp.ViewModel;
using Autofac;

namespace AevApp.Plumbing.IoC
{
    public class MvvmModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterModels(builder);

            RegisterViewModels(builder);

            RegisterViews(builder);
        }

        private void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<HomeViewModel>()
                .AsSelf()
                .PropertiesAutowired();

            builder.RegisterType<OfficerDashboardViewModel>()
                .AsSelf()
                .PropertiesAutowired();

            builder.RegisterType<SecurityCheckSubmissionViewModel>()
                .AsSelf()
                .PropertiesAutowired();

            builder.RegisterType<SetupViewModel>()
                .AsSelf()
                .PropertiesAutowired();

            builder.RegisterType<AddVpViewModel>()
                .AsSelf()
                .PropertiesAutowired();

            builder.RegisterType<RestartAgainViewModel>()
                .AsSelf()
                .PropertiesAutowired();
        }

        private void RegisterModels(ContainerBuilder builder)
        {
        }

        private void RegisterViews(ContainerBuilder builder)
        {
            builder.RegisterType<HomePage>()
                .AsSelf();

            builder.RegisterType<OfficerDashboardPage>()
                .AsSelf();

            builder.RegisterType<SecurityCheckSubmissionPage>()
                .AsSelf();

            builder.RegisterType<SecurityCheckSubmissionPage_Portrait>();

            builder.RegisterType<SetupPage>()
                .AsSelf();

            builder.RegisterType<AddVpPage>()
                .AsSelf();

            builder.RegisterType<RestartAgainPage>()
                .AsSelf();
        }
    }
}