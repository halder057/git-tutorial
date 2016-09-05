using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BootstrapTest.Startup))]
namespace BootstrapTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
