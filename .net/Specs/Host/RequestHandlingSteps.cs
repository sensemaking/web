﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Bdd;
using Sensemaking.Bdd.Web;
using Sensemaking.Http.Json.Client;
using Sensemaking.Web.Api;

namespace Sensemaking.Host.Web.Specs
{
    public partial class RequestHandlingSpecs
    {
        private const string the_route_value = "route_value";
        private const string the_query_value = "query_value";
        private FakePayload the_payload;

        protected override void before_each()
        {
            base.before_each();
            the_payload = null;
        }

        private void a_url() { }

        private void a_route_value() { }

        private void a_query_value() { }

        private void a_payload()
        {
            the_payload = new FakePayload("The payload contents");
        }

        private void a_get_handler_for_the_url() { }

        private void a_put_handler_for_the_url() { }

        private void a_delete_handler_for_the_url() { }

        private void a_post_handler_for_the_url() { }

        private void getting()
        {
            get<FakeGetter.Response>(GenerateUrl(FakeGetter.Url));
        }

        private void putting()
        {
            put(GenerateUrl(FakePutter.Url), the_payload);
        }

        private void deleting()
        {
            delete(GenerateUrl(FakeDeleter.Url));
        }

        private void posting()
        {
            post(GenerateUrl(FakePoster.Url), the_payload);
        }

        private void the_get_handler_processes_the_request()
        {
            it_is_ok();
            the_response_body<FakeGetter.Response>().QueryValue.should_be(the_query_value);
            the_response_body<FakeGetter.Response>().RouteValue.should_be(the_route_value);
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

        private string GenerateUrl(string url)
        {
            return $"{url}/{the_route_value}?{FakeKeys.QueryKey}={the_query_value}";
        }
    }
}
