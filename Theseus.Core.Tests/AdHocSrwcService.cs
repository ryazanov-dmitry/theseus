using System.Collections.Generic;
using System.Threading.Tasks;
using Theseus.Core.Dto;

namespace Theseus.Core.Tests
{
    /// <summary>
    /// Fake short range wireless communication service.
    /// </summary>
    internal class AdHocSrwcService : ISrwcService, IMediator
    {
        public List<INode> colleagues { get; private set; }

        /// <summary>
        /// This is dummy Broadcast. It imitates that srwc middleware classifies messages, which implies
        /// Node's interface will be called with particular method like ReceiveDKG or ReceiveBeacon.
        /// I guess its right...?
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Broadcast(object message)
        {

            if (message is DKGRequest dKGRequest)
            {
                foreach (var node in this.colleagues)
                {
                    await node.ReceiveDKG(dKGRequest);
                }
            }

            if (message is Beacon beacon)
            {
                foreach (var node in this.colleagues)
                {
                    node.ReceiveBeacon(beacon);
                }
            }
        }

        public void RegisterColleagues(List<INode> colleagues)
        {
            this.colleagues = colleagues;
        }
    }
}