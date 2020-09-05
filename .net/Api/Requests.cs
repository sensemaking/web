using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;

namespace Sensemaking.Web.Api
{
    public class Request
    {
        public Request(IDictionary<string, object> values)
        {
            Values = new ReadOnlyDictionary<string, object>(values);
        }

        public object this[string key] => Values[key];
        public IReadOnlyDictionary<string, object> Values { get; }
    }

    public interface IHandleGetRequests
    {
        string Route { get; }  
        Task<object> Handle(Request request);
    }

    public interface IHandleDeleteRequests 
    {
        string Route { get; }  
        Task<HttpStatusCode> Handle(Request request);
    }

    public interface IRequestCommandHandler
    {
        string Route { get; }
    }

    public interface IRequestCommandHandler<in T> : IRequestCommandHandler
    {
        Task<HttpStatusCode> Handle(Request request, T payload);
    }

    public interface IPutRequestHandler : IRequestCommandHandler {}
    public interface IRequestPostHandler : IRequestCommandHandler {}

    public interface IHandlePutRequests<in T> : IPutRequestHandler, IRequestCommandHandler<T> { }

    public interface IHandlePostRequests<in T> : IRequestPostHandler, IRequestCommandHandler<T> { }
}

