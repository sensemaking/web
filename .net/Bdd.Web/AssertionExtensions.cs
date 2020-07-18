using System;
using System.Net;
using Sensemaking.Http;
using Sensemaking.Http.Json.Client;

namespace Sensemaking.Bdd.Web
{
    public static class AssertionExtensions
    {
        public static void should_be_ok(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.OK);
            response.should_have_content_type();
        }

        public static void should_be_no_content(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.NoContent);
            response.should_have_no_content_type();
        }

        public static void should_be_accepted(this JsonResponse response)
        {
            response.Status.Code.should_be(HttpStatusCode.Accepted);
            response.should_have_no_content_type();
        }

        public static void should_be_not_found(this ProblemException problem)
        {
            problem.should_have_problem(HttpStatusCode.NotFound);
        }

        public static void should_be_forbidden(this ProblemException problem)
        {
            problem.should_have_problem(HttpStatusCode.Forbidden);
        }

        public static void should_be_unauthorised(this ProblemException problem)
        {
            problem.should_have_problem(HttpStatusCode.Unauthorized);
        }

        public static void should_be_not_acceptable(this ProblemException problem)
        {
            problem.should_have_problem(HttpStatusCode.NotAcceptable);
        }

        public static void should_be_service_unavailable(this ProblemException problem)
        {
            problem.should_have_problem(HttpStatusCode.ServiceUnavailable);
        }

        public static void should_be_internal_error(this ProblemException problem)
        {
            problem.should_have_problem(HttpStatusCode.InternalServerError);
        }

        public static void should_be_bad_request(this ProblemException problem, params string[] messages)
        {
            problem.should_have_problem(HttpStatusCode.BadRequest, messages);
        }

        public static void should_be_conflict(this ProblemException problem, params string[] messages)
        {
            problem.should_have_problem(HttpStatusCode.Conflict, messages);
        }

        private static void should_have_content_type(this JsonResponse response)
        {
            response["Content-Type"].should_not_be_empty();
        }

        private static void should_have_no_content_type(this JsonResponse response)
        {
            response["Content-Type"].should_be_empty();
        }

        public static void should_have_problem(this ProblemException problem, HttpStatusCode code, params string[] messages)
        {
            if (problem == null)
                "it".should_fail("Problem exception was not provided.");

            problem.Status.Code.should_be(code);
            if(messages.None())
                problem.Problem.should_be(Problem.Empty);
            else
                problem.Problem.Errors.should_be(messages);
        }
    }
}