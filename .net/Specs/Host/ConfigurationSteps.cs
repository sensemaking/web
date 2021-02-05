using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Bdd;

namespace Sensemaking.Host.Web.Specs
{
    public partial class ConfigurationSpecs
    {
        private void app_settings_are_provided()
        {
            var configuration = Sensemaking.Web.Host.HostBuilder.Create<SpecificationStartup>(Array.Empty<string>()).Build()
                .Services.GetServices<IConfiguration>().Single();

            configuration["TheAppSetting"].should_be("TheValue");
        }

        private void any_further_configuration_is_provided()
        {
            var configuration = Sensemaking.Web.Host.HostBuilder.Create<SpecificationStartup>(
                    Array.Empty<string>(),
                    (builder) => builder.AddJsonFile("appsettings-additional.json", false, true))
                .Build()
                .Services.GetServices<IConfiguration>().Single();

            configuration["TheAdditionalSetting"].should_be("TheAdditionalValue");
        }
    }
}
