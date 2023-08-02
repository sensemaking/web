using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ClearExtensions;
using Sensemaking.Bdd;
using Sensemaking.Host.Monitoring;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web.Specs
{
    public partial class IsAliveSpecs
    {
        private static readonly MonitoringAlert alert = AlertFactory.ServiceUnavailable(new MonitorInfo("Monitor", "For alert", "Some Instance"), "We is down");

        private void monitor_has_full_service()
        {
            startup.SetMonitorAvailability(Availability.Up());
        }

        private void monitor_has_reduced_service()
        {
            startup.SetMonitorAvailability(Availability.Up() | Availability.Down(alert));
        }

        private void monitor_has_no_service()
        {
            startup.SetMonitorAvailability(Availability.Down(alert));
        }

        private void checking_service_availability()
        {
            trying(getting<IsAliveResponse>("is-alive"));
        }

        private void status_is_up()
        {
            the_response_body<IsAliveResponse>().Status.should_be("Service is up!");
        }
    }

    public class IsAliveResponse
    {
        public string Status { get; private set; }
    }

    public class IsAliveStartup : SpecificationStartup
    {
        public IsAliveStartup(IConfiguration configuration) : base(configuration) { }

        protected override IMonitorServices ServiceMonitor { get; } = Substitute.For<IMonitorServices>();

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton(ServiceMonitor);
            SetMonitorAvailability(Availability.Up());
        }

        public void SetMonitorAvailability(Availability availability)
        {
            ServiceMonitor.ClearSubstitute();
            ServiceMonitor.Availability().Returns(availability);
        }
    }
}