using System;
using System.Threading.Tasks;
using Theseus.Core;
using Theseus.Core.Dto;
using Theseus.Core.Infrastructure;

namespace Theseus.Core.Tests
{
    public class FakeNodeGateway : INodeGateway
    {
        public INode Node { get; set; }

        public FakeNodeGateway(INode node)
        {
            this.Node = node;
        }

        public async Task Receive(object message)
        {
            if (message is DKGRequest dKGRequest)
            {
                await Node.ReceiveDKG(dKGRequest);
            }

            if (message is Beacon beacon)
            {
                Node.ReceiveBeacon(beacon);
            }

            if (message is DKGInitRequest dkgInitRequest)
            {
                await Node.DKGClient.ReceiveTryInitDKGSession(dkgInitRequest);
            }

            if (message is DKGStartSessionAccept dkgStartSessionAccept)
            {
                await Node.DKGClient.ReceiveDKGStartSessionAccept(dkgStartSessionAccept);
            }

            if (message is DKGSessionList dkgSessionList)
            {
                await Node.DKGClient.ReceiveDKGSessionList(dkgSessionList);
            }
        }
    }
}