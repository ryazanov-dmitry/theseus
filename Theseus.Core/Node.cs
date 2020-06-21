using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theseus.Core
{
    public class Node
    {
        public string Id { get; }

        private readonly ISrwcService srwcService;

        public Node(ISrwcService srwcService)
        {
            this.Id = GenerateId();
            this.srwcService = srwcService;
        }


        public async Task BroadcastPersonalBeacon()
        {
            var message = CreateBeaconMessage();
            await srwcService.Broadcast(message);
        }

        public async Task RequestDKG(string proverNodeId)
        {
            var message = CreateDKGMessage(proverNodeId);
            await srwcService.Broadcast(message);
        }

        public List<string> GetDKGPubs()
        {
            throw new NotImplementedException();
        }

        public void ReceiveBeacon(Beacon beaconMessage)
        {
            throw new NotImplementedException();
        }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        private Message CreateDKGMessage(string proverNodeId)
        {
            return new DkgMessage
            {
                ProverNodeId = proverNodeId,
                MessageType = MessageType.DKG
            };
        }

        private Beacon CreateBeaconMessage()
        {
            return new Beacon
            {
                Id = Id
            };
        }

    }
}