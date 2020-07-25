using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ClearExtensions;
using Sensemaking.Bdd;
using Sensemaking.Bdd.Web;
using Sensemaking.Host.Monitoring;
using Sensemaking.Http.Json.Client;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web.Specs
{
    public partial class IsAliveSpecs
    {
        private static readonly Alert alert =
            AlertFactory.ServiceUnavailable(new MonitorInfo("Monitor", "For alert", "Some Instance"), "We is down");

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
            get<IsAliveResponse>("is-alive");
        }

        private void status_is_up()
        {
            (the_response as JsonResponse<IsAliveResponse>).Body.Status.should_be("Service is up!");
        }
    }

    public class IsAliveResponse
    {
        public string Status { get; private set; }
    }

    public class IsAliveStartup : FakeStartup
    {
        internal readonly IMonitorServices Monitor = Substitute.For<IMonitorServices>();

        public IsAliveStartup()
        {
            Monitor.Availability().Returns(Availability.Up());
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton(Monitor);
            SetMonitorAvailability(Availability.Up());
        }

        public void SetMonitorAvailability(Availability availability)
        {
            Monitor.ClearSubstitute();
            Monitor.Availability().Returns(availability);
        }
    }
}