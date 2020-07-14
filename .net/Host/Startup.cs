using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Sensemaking.Web.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.RemoveSupportForTls11AndLower();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapDelete("/", context => Task.CompletedTask ));
        }
    }

    internal static class ApplicationBuilderExtensions
    {
        internal static void RemoveSupportForTls11AndLower(this IApplicationBuilder app)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
        }
    }
}
