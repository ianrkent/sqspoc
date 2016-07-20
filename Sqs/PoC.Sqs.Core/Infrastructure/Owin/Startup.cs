using Nancy.Owin;
using Owin;

namespace PoC.Sqs.Core.Infrastructure.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(new NancyOptions { Bootstrapper = new CustomAutofacNancyBootstrapper() });
        }
    }
}