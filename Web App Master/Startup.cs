using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web_App_Master.Startup))]
namespace Web_App_Master
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
