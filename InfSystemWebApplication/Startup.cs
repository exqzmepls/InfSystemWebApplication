using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InfSystemWebApplication.Startup))]
namespace InfSystemWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
