using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Hosting.Host;

namespace Sensemaking.Host.Web
{
    public static class HostBuilder
    {
        public static IHostBuilder Create(string[] args)
        {
            return CreateDefaultBuilder(args)
                .ConfigureLogging(logging => logging.ClearProviders())
                .ConfigureWebHostDefaults(builder => builder.UseStartup<JsonApiStartup>())
                .ConfigureWebHost(b => b.UseWebRoot("public"));
        }
    }
}
