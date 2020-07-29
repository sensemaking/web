using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.TestHost;
using NSubstitute.ClearExtensions;
using Sensemaking.Http.Json.Client;

namespace Sensemaking.Bdd.Web
{
    public abstract partial class Specification<T> : Specification where T : FakeStartup, new()
    {
        protected static readonly T startup;
        protected static readonly IServiceProvider services;
        protected static FlurlClient client;

        protected JsonResponse the_response;
        protected U the_response_body<U>() => (the_response as JsonResponse<U>).Body;

        protected ProblemException the_problem_exception;

        static Specification()
        {
            startup = new T();
            var factory = new WebApplicationFactory(startup).WithWebHostBuilder(b => b.UseSolutionRelativeContentRoot("./Host"));
            client = new FlurlClient(factory.CreateClient());
            services = factory.Services;
        }

        protected override void before_each()
        {
            base.before_each();
            the_response = null;
            the_problem_exception = null;
        }

        protected override void after_each()
        {
            startup.SubstituteLogger.ClearSubstitute();
            base.after_all();
        }

        protected override void trying(Action action)
        {
            base.trying(action);
            the_problem_exception = the_exception as ProblemException;
        }

        protected virtual void get<U>(string url, params (string Name, string Value)[] headers)
        {
            the_response = client.GetAsync<U>(url, headers).Result;
        }

        protected virtual void put(string url, object payload, params (string Name, string Value)[] headers)
        {
            the_response = client.PutAsync(url, payload, headers).Result;
        }

        protected virtual void delete(string url, params (string Name, string Value)[] headers)
        {
            the_response = client.DeleteAsync(url, headers).Result;
        }

        protected virtual void post(string url, object payload, params (string Name, string Value)[] headers)
        {
            the_response = client.PostAsync(url, payload, headers).Result;
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
            the_problem_exception.should_be_forbidden();
        }

        public void it_is_unauthorised()
        {
            the_problem_exception.should_be_unauthorised();
        }

        public void it_is_not_acceptable()
        {
            the_problem_exception.should_be_not_acceptable();
        }

        public void it_is_not_found()
        {
            the_problem_exception.should_be_not_found();
        }

        public void it_is_service_unavailable()
        {
            the_problem_exception.should_be_service_unavailable();
        }

        public void it_is_an_internal_error()
        {
            the_problem_exception.should_be_internal_error();
        }

        public void it_is_a_bad_request(params string[] erros)
        {
            the_problem_exception.should_be_bad_request(erros);
        }

        public void it_is_a_conflict(params string[] messages)
        {
            the_problem_exception.should_be_conflict(messages);
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