using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdminPanel.Startup))]
namespace AdminPanel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
