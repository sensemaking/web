using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Hosting.Host;

namespace Sensemaking.Web.Host
{
    public static class HostBuilder
    {
        public static IHostBuilder Create<T>(string[] args) where T : ApiStartup
        {
            return Create<T>(args, (builder) => { });
        }

        public static IHostBuilder Create<T>(string[] args, Action<IConfigurationBuilder> builder) where T : ApiStartup
        {
            return CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true);
                    builder(config);
                })
                .ConfigureLogging(logging => logging.ClearProviders())
                .ConfigureWebHostDefaults(builder => builder.UseStartup<T>())
                .ConfigureWebHost(b => b.UseWebRoot("public"));
        }
    }
}
