using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Bdd;
using Sensemaking.Web.Host;

namespace Sensemaking.Host.Web.Specs
{
    public partial class RequestHandlingSpecs
    {
        private const string the_route_value = "route_value";
        private const string the_query_value = "query_value";
        private const string the_header_value = "header_value";
        private FakePayload the_payload;

        protected override void before_each()
        {
            base.before_each();
            the_payload = null;
        }

        private void a_url() { }

        private void a_route_value() { }

        private void a_query_value() { }

        private void a_header_value() { }

        private void a_pipeline_injected_value() { }

        private void a_payload()
        {
            the_payload = new FakePayload("The payload contents");
        }

        private void a_get_handler_for_the_url() { }

        private void a_put_handler_for_the_url() { }

        private void a_delete_handler_for_the_url() { }

        private void a_post_handler_for_the_url() { }

        private void a_custom_request_factory() { }

        private void getting()
        {
            var (url, header) = generate_request(FakeGetter.Url);
            get<FakeGetter.Response>(url, header);
        }

        private void putting()
        {
            var (url, header) = generate_request(FakePutter.Url);
            put(url, the_payload, header);
        }

        private void deleting()
        {
            var (url, header) = generate_request(FakeDeleter.Url);
            delete(url, header);
        }

        private void posting()
        {
            var (url, header) = generate_request(FakePoster.Url);
            post(url, the_payload, header);
        }

        private void making_a_request()
        {
            getting();
        }

        private void the_get_handler_processes_the_request()
        {
            it_is_ok();
            the_response_body<FakeGetter.Response>().QueryValue.should_be(the_query_value);
            the_response_body<FakeGetter.Response>().RouteValue.should_be(the_route_value);
            the_response_body<FakeGetter.Response>().HeaderValue.should_be(the_header_value);
            the_response_body<FakeGetter.Response>().PipelineValue.should_be(startup.PipelineValue);
        }

        private void the_put_handler_processes_the_request()
        {
            the_response.Status.should_be(FakePutter.ResponseStatusCode);
        }

        private void the_delete_handler_processes_the_request()
        {
            the_response.Status.should_be(FakeDeleter.ResponseStatusCode);
        }

        private void the_post_handler_processes_the_request()
        {
            the_response.Status.should_be(FakePoster.ResponseStatusCode);
        }

        private void the_custom_factory_is_used()
        {
            startup.OnlyFakeFactoryRegistered.should_be_true();
        }

        private static (string Url, (string Name, string Value) Header) generate_request(string url)
        {
            return ($"{url}/{the_route_value}?{FakeKeys.QueryKey}={the_query_value}", (FakeKeys.HeaderKey, the_header_value));
        }
    }


    public class RequestHandlingStartup : SpecificationStartup
    {
        public RequestHandlingStartup(IConfiguration configuration) : base(configuration) { }

        public string PipelineValue = "SomePipelineValue";

        public bool OnlyFakeFactoryRegistered { get; private set; }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.ProvideRequestCreation(new FakeRequestFactory());
            var exceptionHandlers = services.BuildServiceProvider().GetServices<RequestFactory>().ToArray();
            OnlyFakeFactoryRegistered = exceptionHandlers.Count() == 1 && exceptionHandlers.All(h => h.GetType() == typeof(FakeRequestFactory));
        }

        protected override IApplicationBuilder AdditionalMiddleware(IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                context.Features.Set(new FakeFeature(PipelineValue));
                return next();
            });

            return app;
        }
    }


}
