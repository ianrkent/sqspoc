using Autofac;
using Nancy.Bootstrappers.Autofac;

namespace PoC.Sqs.Core.Infrastructure.Owin
{
    public class CustomAutofacNancyBootstrapper : AutofacNancyBootstrapper
    {
        protected override ILifetimeScope GetApplicationContainer()
        {
            return IoCConfig.Container;
        }
    }
}