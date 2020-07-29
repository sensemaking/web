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

    public interface IHandleCommands
    {
        string Route { get; }
        HttpStatusCode Handle();
    }

    public interface IHandlePutRequests : IHandleCommands {}
    public interface IHandlePostRequests : IHandleCommands {}
    public interface IHandleDeleteRequests : IHandleCommands {}
}
