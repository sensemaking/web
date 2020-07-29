using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Hosting.Host;

namespace Sensemaking.Web.Host
{
    public static class HostBuilder
    {
        public static IHostBuilder Create<T>(string[] args) where T : JsonApiStartup
        {
            return CreateDefaultBuilder(args)
                .ConfigureLogging(logging => logging.ClearProviders())
                .ConfigureWebHostDefaults(builder => builder.UseStartup<T>())
                .ConfigureWebHost(b => b.UseWebRoot("public"));
        }
    }
}
