using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theseus.Core.Tests
{
    internal class AdHocSrwcService : ISrwcService, IMediator
    {
        public List<IColleagues> colleagues { get; private set; }

        public Task Broadcast(object message)
        {
            return null;
            // foreach (var node in colleagues)
            // {
            //     node.
            // }
        }

        public void RegisterColleagues(List<IColleagues> colleagues)
        {
            this.colleagues = colleagues;
        }
    }
}