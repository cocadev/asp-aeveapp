using AevApp.Plumbing.Mapper;
using Autofac;
using AutoMapper;

namespace AevApp.Plumbing.IoC
{
    public class MapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(context => new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MapperProfile>();
                }
            )).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();
        }
    }
}