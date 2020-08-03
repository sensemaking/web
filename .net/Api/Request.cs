using System.Net;
using System.Net.Http;
using System.Serialization;
using System.Threading.Tasks;

namespace Sensemaking.Web.Api
{
    public interface IHandleGetRequests
    {
        string Route { get; }  
        Task<object> Handle();
    }

    public interface IRequestCommandHandler
    {
        string Route { get; }
        Task<HttpStatusCode> HandleJson(string json);
    }

    public interface IRequestCommandHandler<in T> : IRequestCommandHandler
    {
        Task<HttpStatusCode> IRequestCommandHandler.HandleJson(string json)
        {
            return Handle(json.Deserialize<T>());
        }

        Task<HttpStatusCode> Handle(T request);
    }

    public interface IPutRequestHandler : IRequestCommandHandler {}
    public interface IHandlePutRequests<in T> : IPutRequestHandler, IRequestCommandHandler<T> {}
    public interface IRequestDeleteHandler : IRequestCommandHandler {}
    public interface IHandleDeleteRequests<in T> : IRequestDeleteHandler, IRequestCommandHandler<T> {}
    public interface IRequestPostHandler : IRequestCommandHandler {}
    public interface IHandlePostRequests<in T> : IRequestPostHandler, IRequestCommandHandler<T> {}
}
