using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;

namespace Theseus.Core
{
    public interface INode
    {
        string Id { get; }

        Task BroadcastPersonalBeacon();
        List<string> GetDKGPubs();
        void ReceiveBeacon(Beacon beaconMessage);
        Task RequestDKG(string proverNodeId);
    }

    public class Node : INode
    {
        public string Id { get; }

        private readonly ISrwcService srwcService;
        private readonly IAuthentication authentication;

        public Node(ISrwcService srwcService, IAuthentication authentication)
        {
            this.Id = GenerateId();
            this.authentication = authentication;
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

        public virtual void ReceiveBeacon(Beacon beaconMessage)
        {
            authentication.Verify(beaconMessage);
            
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