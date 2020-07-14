using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.TestHost;
using Sensemaking.Host.Web;
using Sensemaking.Http;

namespace Sensemaking.Bdd.Web
{
    public abstract partial class Specification<T> : Specification where T : Startup, new()
    {
        private static readonly string root_url;

        protected static readonly T startup = new T();
        protected static readonly IServiceProvider services;
        protected JsonResponse the_response;

        static Specification()
        {
            var factory = new WebApplicationFactory().WithWebHostBuilder(b => b.UseSolutionRelativeContentRoot(".\\Host"));
            services = factory.Services;
            var client = factory.CreateClient();
            root_url = client.BaseAddress.AbsoluteUri;
            FlurlHttp.Configure(settings => settings.HttpClientFactory = new HttpClientFactory(client));
        }

        protected override void before_each()
        {
            base.before_each();
            the_response = null;
        }

        protected async Task get<U>(string url, params (string Name, string Value)[] headers)
        {
            the_response = await root_url.WithSegment(url).GetAsync<U>(headers);
        }

        protected async Task delete(string url, params (string Name, string Value)[] headers)
        {
            the_response = await root_url.WithSegment(url).DeleteAsync(headers);
        }

        protected async Task put(string url, object payload, params (string Name, string Value)[] headers)
        {

            the_response = await root_url.WithSegment(url).PutAsync(payload, headers);
        }

        protected async Task post(string url, object payload, params (string Name, string Value)[] headers)
        {

            the_response = await root_url.WithSegment(url).PostAsync(payload, headers);
        }

        public void it_is_ok()
        {
            the_response.should_be_ok();
        }

        public void it_is_no_content()
        {
            the_response.should_be_no_content();
        }

        public void it_is_accepted()
        {
            the_response.should_be_accepted();
        }

        public void it_is_forbidden()
        {
            the_response.should_be_forbidden();
        }

        public void it_is_unauthorised()
        {
            the_response.should_be_unauthorised();
        }

        public void it_is_not_acceptable()
        {
            the_response.should_be_not_acceptable();
        }

        public void it_is_a_bad_request(params string[] messages)
        {
            the_response.should_be_bad_request(messages);
        }

        public void it_is_not_found()
        {
            the_response.should_be_not_found();
        }

        public void it_is_service_unavailable(string message)
        {
            the_response.should_be_service_unavailable();
        }

        public void it_is_an_internal_error(string message)
        {
            the_response.should_be_internal_error();
        }

        public void it_is_a_conflict(string message)
        {
            the_response.should_be_conflict(message);
        }
    }
    
    public static class Extensions
    {
        public static string WithSegment(this string root, string path)
        {
            return root.AppendPathSegment(path);
        }
    }
}