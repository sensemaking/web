using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Sensemaking.Host.Web
{
    internal static class Logger
    {
        internal static IApplicationBuilder UseLogger(this IApplicationBuilder app, ILogger logger)
        {
            Logging.Configure(logger);
            return app;
        }
    } 
}