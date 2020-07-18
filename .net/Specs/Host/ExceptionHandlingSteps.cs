using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Sensemaking.Bdd;
using Sensemaking.Http;
using Sensemaking.Http.Json.Client;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web.Specs
{
    public partial class ExceptionHandlingSpecs
    {
        private static readonly NotAllowedException not_allowed_exception = new NotAllowedException();
        private static readonly NotFoundException not_found_exception = new NotFoundException();
        private static readonly ServiceAvailabilityException service_availability_exception = new ServiceAvailabilityException();
        private static readonly Exception unexpected_exception = new Exception();
        private static readonly ValidationException validation_exception = new ValidationException("That ain't so good.", "And neither is this.", "Nor this.");
        private static readonly ConflictException conflict_exception = new ConflictException("Thingy caused a conflict.");


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
            get<object>(ExceptionStartup.exception_throwing_url);
        }

        public void it_has_no_content_type()
        {
            the_exception.Headers.ValueFor("Content-Type").should_be_empty();
        }

        public void it_has_problem_content_type()
        {
            the_exception.Headers.ValueFor("Content-Type").should_be(MediaType.JsonProblem);
        }
    }

    public class ExceptionStartup : JsonApiStartup
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