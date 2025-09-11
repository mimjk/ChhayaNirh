using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;

[assembly: OwinStartup(typeof(ChhayaNirh.Startup))]

namespace ChhayaNirh
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure SignalR
            var hubConfiguration = new HubConfiguration()
            {
                EnableDetailedErrors = true
            };

            app.MapSignalR(hubConfiguration);
        }
    }
}