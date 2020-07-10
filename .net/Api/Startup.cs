using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            RemoveSupportForTls11AndLower();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapDelete("/", context => Task.CompletedTask ));
        }

        private void RemoveSupportForTls11AndLower()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
        }
    }
}
