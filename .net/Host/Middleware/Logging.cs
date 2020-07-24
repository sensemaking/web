﻿using System;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Sensemaking.Host.Web
{
    internal static class Logging
    {
        internal static IApplicationBuilder UseLogger(this IApplicationBuilder app, ILogger logger)
        {
            Sensemaking.Logging.Configure(logger);
            return app;
        }
    } 
}