using Autofac;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using Nancy.Owin;
using Owin;

namespace PoC.Sqs.Core.Infrastructure.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IoCConfig.BuildContainer();

            //app.UseNancy(new NancyOptions { Bootstrapper = new DefaultNancyBootstrapper() });
            app.UseNancy(new NancyOptions { Bootstrapper = new CustomNancyBoots() });
        }
    }

    public class CustomNancyBoots : AutofacNancyBootstrapper
    {
        protected override ILifetimeScope GetApplicationContainer()
        {
            return IoCConfig.Container;
        }
    }
}