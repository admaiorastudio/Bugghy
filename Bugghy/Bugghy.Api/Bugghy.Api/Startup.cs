[assembly: Microsoft.Owin.OwinStartup(typeof(AdMaiora.Bugghy.Api.Startup))]

namespace AdMaiora.Bugghy.Api
{
    using Microsoft.Owin;
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}