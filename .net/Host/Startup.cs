using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Host.Web
{
    public class Startup
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
