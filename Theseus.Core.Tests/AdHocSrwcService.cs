using System.Collections.Generic;
using System.Threading.Tasks;
using Theseus.Core.Dto;

namespace Theseus.Core.Tests
{
    /// <summary>
    /// Short range wireless communication service.
    /// </summary>
    internal class AdHocSrwcService : ISrwcService, IMediator
    {
        public List<INode> colleagues { get; private set; }

        public async Task Broadcast(object message)
        {

            if (message is DKGRequest)
            {
                foreach (var node in this.colleagues)
                {
                    await node.ReceiveDKG((DKGRequest)message);
                }
            }

        }

        public void RegisterColleagues(List<INode> colleagues)
        {
            this.colleagues = colleagues;
        }
    }
}