using System.Net.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Sensemaking.Bdd.Web
{
    public abstract partial class Specification<T>
    {
        private class WebApplicationFactory : WebApplicationFactory<T>
        {
            protected override IWebHostBuilder CreateWebHostBuilder()
            {
                return WebHost.CreateDefaultBuilder().UseStartup<T>();
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