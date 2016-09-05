using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpiderDashboard.Startup))]
namespace SpiderDashboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
