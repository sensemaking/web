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
            response.Status.should_be(HttpStatusCode.OK);
        }

        public static void should_be_no_content(this JsonResponse response)
        {
            response.Status.should_be(HttpStatusCode.NoContent);
        }

        public static void should_be_accepted(this JsonResponse response)
        {
            response.Status.should_be(HttpStatusCode.Accepted);
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

        public static void should_be_bad_request(this ProblemException problem, string problemTitle, params string[] errors)
        {
            problem.should_have_problem(HttpStatusCode.BadRequest, problemTitle, errors);
        }

        public static void should_be_conflict(this ProblemException problem, params string[] errors)
        {
            problem.should_have_problem(HttpStatusCode.Conflict, "Fulfilling the request would cause a conflict.", errors);
        }

        public static void should_have_problem(this ProblemException problem, HttpStatusCode code, string problemTitle = "", params string[] errors)
        {
            if (problem == null)
                "it".should_fail("Problem exception was not provided.");

            problem.Status.should_be(code);
            if (errors.None())
                problem.Problem.Errors.should_be_empty();
            else
            {
                problem.Problem.Title.should_be(problemTitle);
                problem.Problem.Errors.should_be(errors);
            }
        }
    }
}