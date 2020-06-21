using System.Threading.Tasks;

namespace Theseus.Core
{
    public interface ISrwcService
    {
        Task Broadcast(object v);
    }
}