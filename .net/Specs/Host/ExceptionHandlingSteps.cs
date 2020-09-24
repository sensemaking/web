using System;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;
using NSubstitute.ClearExtensions;
using Sensemaking.Bdd;
using Sensemaking.Bdd.Web;
using Sensemaking.Http;
using Sensemaking.Http.Json.Client;
using Sensemaking.Monitoring;
using Sensemaking.Web.Host;
using Serilog;

namespace Sensemaking.Host.Web.Specs
{
    public partial class ExceptionHandlingSpecs
    {
        private static readonly WhoAreYouException who_are_you_exception = new WhoAreYouException();
        private static readonly AccessException not_allowed_exception = new AccessException();
        private static readonly NotFoundException not_found_exception = new NotFoundException();
        private static readonly ServiceAvailabilityException service_availability_exception = new ServiceAvailabilityException();
        private static readonly Exception unexpected_exception = new Exception();
        private static readonly ValidationException validation_exception = new ValidationException("That ain't so good.", "And neither is this.", "Nor this.");
        private static readonly ConflictException conflict_exception = new ConflictException("Thingy caused a conflict.");
        private string logged_alert;

        protected override void before_each()
        {
            base.before_each();
            logged_alert = null;
            startup.SubstituteLogger.When(l => l.Error(Arg.Any<string>())).Do(c => logged_alert = c.Arg<string>());
        }

        protected override void after_each()
        {
            base.after_each();
            startup.SubstituteLogger.ClearSubstitute();
        }

        private void a_(Exception exception)
        {
            startup.CauseException(exception);
        }

        private void an_(Exception exception)
        {
            a_(exception);
        }

        private void handling_a_request()
        {
           trying(() => get<object>(ExceptionStartup.exception_throwing_url));
        }

        public void it_has_no_content_type()
        {
            the_problem_exception.Headers.ValueFor("Content-Type").should_be_empty();
        }

        private void it_logs(Alert alert)
        {
            logged_alert.should_be(alert.Serialize());
        }
    }

    public class ExceptionStartup : SpecificationStartup
    {
        public const string exception_throwing_url = "/throw";
        private static Exception exception;

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
            app.UseEndpoints(endpoints => endpoints.MapGet(exception_throwing_url, context => throw exception));
        }

        public void CauseException(Exception exception) { ExceptionStartup.exception = exception; }
    }
}