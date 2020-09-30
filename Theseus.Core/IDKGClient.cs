using System.Threading.Tasks;

namespace Theseus.Core
{
    public interface IDKGClient
    {
        Task Generate();
    }
}