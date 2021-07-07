using System.Threading.Tasks;

namespace Theseus.Core.Infrastructure
{
    // TODO: Move to API layer?
    public interface INodeGateway
    {
        Task Receive(object message);
    }
}