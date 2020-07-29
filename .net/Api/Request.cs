using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sensemaking.Web.Api
{
    public interface IHandleGetRequests
    {
        string Route { get; }  
        Task<object> Handle();
    }

    public interface IHandleCommandRequests
    {
        string Route { get; }
        Task<HttpStatusCode> Handle();
    }

    public interface IHandlePutRequests : IHandleCommandRequests {}
    public interface IHandlePostRequests : IHandleCommandRequests {}
    public interface IHandleDeleteRequests : IHandleCommandRequests {}
}
