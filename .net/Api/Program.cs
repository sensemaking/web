using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Microsoft.Extensions.Hosting.Host;

namespace Api
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}
