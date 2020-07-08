using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Sensemaking.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Sensemaking.Bdd.Web
{
    public abstract class RequestSpecification<T> : Specification where T : class
    {
        private static readonly HttpClient client;
        protected JsonResponse the_response;

        static RequestSpecification() => client = new WebApplicationFactory<T>().CreateClient();

        protected override void before_each()
        {
            base.before_each();
            the_response = null;
        }

        protected async Task get<U>(string url, params (string Name, string Value)[] headers)
        {
            the_response = await client.BaseAddress.WithPath(url).GetAsync<U>(headers);
        }

        protected async Task delete(string url, params (string Name, string Value)[] headers)
        {
            the_response = await client.BaseAddress.WithPath(url).DeleteAsync(headers);
        }

        protected async Task put(string url, object payload, params (string Name, string Value)[] headers)
        {

            the_response = await client.BaseAddress.WithPath(url).PutAsync(payload, headers);
        }

        protected async Task post(string url, object payload, params (string Name, string Value)[] headers)
        {

            the_response = await client.BaseAddress.WithPath(url).PostAsync(payload, headers);
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
        public static string WithPath(this Uri root, string path)
        {
            return root.AbsoluteUri.AppendPathSegment(path);
        }

        public static void AuthenticateUsing(this IDictionary<string, string> headers, string token)
        {
            headers.Add("Authorization", $"Bearer {token}");
        }
    }
}