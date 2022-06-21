using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DrivingSclApp.Startup))]
namespace DrivingSclApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
