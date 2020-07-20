using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Sensemaking.Host.Web;
using Sensemaking.Http;
using Sensemaking.Http.Json.Client;

namespace Sensemaking.Bdd.Web
{
    public abstract partial class Specification<T> : Specification where T : JsonApiStartup, new()
    {
        private static readonly string root_url;

        protected static readonly IServiceProvider services;
        private static FlurlClient client;

        protected T startup = new T();
        protected JsonResponse the_response;
        protected ProblemException the_exception;

        static Specification()
        {
            var factory = new WebApplicationFactory().WithWebHostBuilder(b => b.UseSolutionRelativeContentRoot(".\\Host"));
            client = new FlurlClient(factory.CreateClient());
            services = factory.Services;
        }

        protected override void before_each()
        {
            base.before_each();
            startup = new T();
            the_response = null;
            the_exception = null;
        }

        protected void get<U>(string url, params (string Name, string Value)[] headers)
        {
            try
            {
                the_response = client.GetAsync<U>(url, headers).Result;
            }
            catch (AggregateException ex)
            {
                the_exception = ex.InnerException as ProblemException;
            }
        }

        protected async Task put(string url, object payload, params (string Name, string Value)[] headers)
        {
            the_response = await client.PutAsync(url, payload, headers);
        }

        protected async Task delete(string url, params (string Name, string Value)[] headers)
        {
            the_response = await client.DeleteAsync(url, headers);
        }

        protected async Task post(string url, object payload, params (string Name, string Value)[] headers)
        {
            the_response = await client.PostAsync(url, payload, headers);
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
            the_exception.should_be_forbidden();
        }

        public void it_is_unauthorised()
        {
            the_exception.should_be_unauthorised();
        }

        public void it_is_not_acceptable()
        {
            the_exception.should_be_not_acceptable();
        }

        public void it_is_not_found()
        {
            the_exception.should_be_not_found();
        }

        public void it_is_service_unavailable()
        {
            the_exception.should_be_service_unavailable();
        }

        public void it_is_an_internal_error()
        {
            the_exception.should_be_internal_error();
        }

        public void it_is_a_bad_request(params string[] erros)
        {
            the_exception.should_be_bad_request(erros);
        }

        public void it_is_a_conflict(params string[] messages)
        {
            the_exception.should_be_conflict(messages);
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