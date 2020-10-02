using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sensemaking.Web.Api;

namespace Sensemaking.Web.Host
{
    public class RequestFactory
    {
        public Request Create(HttpContext context)
        {
            var dictionaries = new[]
            {
                GetQueryValueFrom(context),
                GetRouteValuesFrom(context),
                GetAdditionalValuesFrom(context.Features)
            };
            
            return new Request(dictionaries.Merge());
        }

        private static IDictionary<string, object> GetQueryValueFrom(HttpContext context)
        {
            return context.Request.Query.ToDictionary(x => x.Key, x => x.Value.First() as object);
        }

        private static IDictionary<string, object> GetRouteValuesFrom(HttpContext context)
        {
            return context.Request.RouteValues;
        }

        protected virtual IDictionary<string, object> GetAdditionalValuesFrom(IFeatureCollection features)
        {
            return new Dictionary<string, object>();
        }
    }

    public static class RequestFactoryExtensions
    {
        public static IServiceCollection ProvideRequestCreation(this IServiceCollection services, RequestFactory factory)
        {
            services.Replace(ServiceDescriptor.Singleton(factory));
            return services;
        }

        internal static IDictionary<string, object> Merge(this IDictionary<string, object>[] dictionaries)
        {
            return dictionaries.SelectMany(dict => dict).ToDictionary(e => e.Key, e => e.Value);
        }
    }
}