using System;
using System.IO;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Sensemaking.Http.Json.Client;
using Sensemaking.Web.Host;

namespace Sensemaking.Bdd.Web
{
    public abstract partial class Specification<T> : Specification where T : ApiStartup
    {
        protected static readonly T startup;
        protected static readonly IServiceProvider services;
        protected static FlurlClient client;

        private JsonResponse response;
        protected JsonResponse the_response {
            get
            {
                if (response == null)
                    throw the_exception;

                return response;
            }
            set => response = value;
        }
        protected U the_response_body<U>() => (the_response as JsonResponse<U>).Body;

        protected ProblemException the_problem_exception;

        static Specification()
        {
            startup = (T)Activator.CreateInstance(typeof(T), new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true).Build() as IConfiguration);

            var factory = new WebApplicationFactory(startup).WithWebHostBuilder(b => b.UseSolutionRelativeContentRoot("./Host"));
            services = factory.Services;

            FlurlHttp.GlobalSettings.AllowedHttpStatusRange = "*";
            client = new FlurlClient(factory.CreateClient());
        }

        protected override void before_each()
        {
            base.before_each();
            the_response = null;
            the_problem_exception = null;
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

        public void it_is_gone()
        {
            the_problem_exception.should_be_gone();
        }

        public void it_is_service_unavailable()
        {
            the_problem_exception.should_be_service_unavailable();
        }

        public void it_is_an_internal_error()
        {
            the_problem_exception.should_be_internal_error();
        }

        public void it_is_a_bad_request(string problemTitle, params string[] errors)
        {
            the_problem_exception.should_be_bad_request(problemTitle, errors);
        }

        public void it_is_a_conflict(params string[] messages)
        {
            the_problem_exception.should_be_conflict(messages);
        }

        public void it_is_unavailable_for_legal_reasons(params string[] messages)
        {
            the_problem_exception.should_be_unavailable_for_legal_reasons(messages);
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