using System.Linq;
using System.Net;
using Sensemaking.Bdd;
using Sensemaking.Http;

namespace System
{
    public static class AssertionExtensions
    {
        public static void should_be_ok(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.OK);
        }

        public static void should_be_no_content(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.NoContent);
            response.should_have_no_content_type();
        }

        public static void should_be_accepted(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.Accepted);
        }

        public static void should_be_forbidden(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.Forbidden);
            response.should_have_no_content_type();
        }

        public static void should_be_unauthorised(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.Unauthorized);
            response.should_have_no_content_type();
        }

        public static void should_be_not_acceptable(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.NotAcceptable);
            response.should_have_no_content_type();
        }

        public static void should_be_bad_request(this JsonResponse response, params string[] messages)
        {
            response.Status.Code.should_be(HttpStatusCode.BadRequest);
            response.should_include_problem(messages);
        }

        public static void should_be_not_found(this JsonResponse response, string message)
        {
            response.Status.Code.should_be(HttpStatusCode.NotFound);
            response.should_include_problem(message);
        }

        public static void should_be_not_found(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.NotFound);
            response.should_have_no_content_type();
        }

        public static void should_be_service_unavailable(this JsonResponse response, string message)
        {
            response.Status.Code.should_be(HttpStatusCode.ServiceUnavailable);
            response.should_include_problem(message);
        }

        public static void should_be_internal_error(this JsonResponse response, string message)
        {
            response.Status.Code.should_be(HttpStatusCode.InternalServerError);
            response.should_include_problem(message);
        }

        public static void should_be_conflict(this JsonResponse response, string message)
        {
            response.Status.Code.should_be(HttpStatusCode.Conflict);
            response.should_include_problem(message);
        }

        private static void should_have_no_content_type(this JsonResponse response)
        {
            response["Content-Type"].should_be_empty();
        }

        private static void should_include_problem(this JsonResponse response, params string[] messages)
        {
            response.Headers.SingleOrDefault(h => h.Name == "Content-Type").Name.should_be($"{MediaType.JsonProblem}; charset=utf-8");
            // var body = response.Body.DeserializeJson<Problem>();
            // body.Title.should_be(response.ToString());
            // messages.ForEach(message => body.Errors.should_contain(message));
            throw new NotImplementedException();
        }
    }
}