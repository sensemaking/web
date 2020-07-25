using System.Net.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace Sensemaking.Bdd.Web
{
    public abstract partial class Specification<T>
    {
        private class WebApplicationFactory : WebApplicationFactory<T>
        {
            private T startup;

            public WebApplicationFactory(T startup)
            {
                this.startup = startup;
            }

            protected override IWebHostBuilder CreateWebHostBuilder()
            {
                return WebHost.CreateDefaultBuilder()
                    .ConfigureLogging(logging => logging.ClearProviders())
                    .ConfigureServices(startup.ConfigureServices)
                    .Configure((context, app) => startup.Configure(app, context.HostingEnvironment));
            }
        }

        private class UseThisClientFactory : DefaultHttpClientFactory
        {
            private readonly HttpClient client;

            public UseThisClientFactory(HttpClient client)
            {
                this.client = client;
            }

            public override HttpClient CreateHttpClient(HttpMessageHandler handler)
            {
                return client;
            }
        }
    }

}