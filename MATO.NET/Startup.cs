using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MATO.NET.Startup))]
namespace MATO.NET
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
