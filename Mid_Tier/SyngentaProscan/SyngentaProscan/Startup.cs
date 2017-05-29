using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SyngentaProscan.Startup))]

namespace SyngentaProscan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}