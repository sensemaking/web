using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Sensemaking.Web.Api
{
    public class Request : ReadOnlyDictionary<string, object>
    {
        public Request(IDictionary<string, object> dictionary) : base(dictionary) { }
    }

    public interface IHandleRequests
    {
        string Route { get; }
        bool AllowUnauthenicatedUsers => false;
    }

    public interface IHandleGetRequests : IHandleRequests
    {        
        Task<object> HandleAsync(Request request);
    }

    public interface IHandleDeleteRequests : IHandleRequests
    {        
        Task<HttpStatusCode> HandleAsync(Request request);
    }

    public interface IRequestCommandHandler<in T> : IHandleRequests 
    {
        Task<HttpStatusCode> HandleAsync(Request request, T payload);
    }

    public class NoBody { }
    public interface IPutRequestHandler : IHandleRequests { }
    public interface IRequestPostHandler : IHandleRequests { }

    public interface IHandlePutRequests<in T> : IPutRequestHandler, IRequestCommandHandler<T> { }

    public interface IHandlePostRequests<in T> : IRequestPostHandler, IRequestCommandHandler<T> { }

    public static class Requests
    {
        public const string UserKey = "AuthenticatedUser";

        public static bool IsAuthenticated(this Request request)
        {
            return request.ContainsKey(UserKey);
        }

        public static ClaimsPrincipal User(this Request request)
        {
            if (!request.ContainsKey(UserKey))
                throw new Exception("You cannot retrieve the user from an unauthenticated request.");

            return (ClaimsPrincipal) request[UserKey];
        }
    }
}

