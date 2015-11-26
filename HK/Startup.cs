using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HK.Startup))]
namespace HK
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
