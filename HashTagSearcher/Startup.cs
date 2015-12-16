using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HashTagSearcher.Startup))]
namespace HashTagSearcher
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
