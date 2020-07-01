using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Microsoft.Extensions.Hosting.Host;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args) => CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
            .Build().Run();
    }
}
