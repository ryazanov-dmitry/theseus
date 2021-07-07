using System.Threading.Tasks;
using Theseus.Core.Navigation;

namespace Theseus.Core
{
    public interface INavigation : IMovable
    {
        void NavigateTo(Coordinates currentClientCoords);
    }
}