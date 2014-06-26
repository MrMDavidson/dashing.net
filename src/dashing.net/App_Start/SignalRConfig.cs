using dashing.net.App_Start;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRConfig))]
namespace dashing.net.App_Start {
    public class SignalRConfig {
        public void Configuration(IAppBuilder app) {
            app.MapSignalR();
        }
    }
}