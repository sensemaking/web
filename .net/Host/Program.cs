using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Microsoft.Extensions.Hosting.Host;

namespace Sensemaking.Host.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
        }
    }
}
