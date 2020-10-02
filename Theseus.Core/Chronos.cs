using System.Threading.Tasks;

namespace Theseus.Core
{
    /// <summary>
    /// This interface is responsible for scheduling jobs for Node
    /// and DKGClient. For example issue DKG participant list 
    /// after 30 seconds after DKG Session was created. Other use cases
    /// comming soon i guess.
    /// </summary>
    public interface IChronos
    {
        Task SetTimeout(Task task, int seconds);
    }



    public class Chronos : IChronos
    {
        public async Task SetTimeout(Task task, int seconds)
        {
            await Task.Delay(seconds * 1000);
            task.RunSynchronously();
        }
    }
}