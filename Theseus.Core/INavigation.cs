using System.Threading.Tasks;

namespace Theseus.Core
{
    public interface INavigation
    {
        Task NavigateTo(Coordinates currentClientCoords);
    }
}