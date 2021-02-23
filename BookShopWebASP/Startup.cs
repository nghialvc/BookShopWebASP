using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookShopWebASP.Startup))]
namespace BookShopWebASP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
