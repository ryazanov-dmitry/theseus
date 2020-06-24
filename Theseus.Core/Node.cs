using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Theseus.Core.Exceptions;

namespace Theseus.Core
{
    public interface INode
    {
        string Id { get; }

        Task BroadcastPersonalBeacon();
        List<string> GetDKGPubs();
        void ReceiveBeacon(Beacon beaconMessage);
        Task ReceiveDKG(DKGRequest message);
        Task RequestDKG(string proverNodeId);
    }

    public class Node : INode
    {
        public string Id => authentication.Base64PublicKey();

        private readonly ISrwcService srwcService;
        private readonly IAuthentication authentication;

        public Node(ISrwcService srwcService, IAuthentication authentication)
        {
            authentication.SetupRSAKeyPairs();
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
            CheckPayloadNodeIdAndPublicKey(beaconMessage);
        }

        public Task ReceiveDKG(DKGRequest dKGRequest)
        {
            throw new NotImplementedException();
        }

        private void CheckPayloadNodeIdAndPublicKey(Beacon beaconMessage)
        {
            if (beaconMessage.Id != System.Convert.ToBase64String(beaconMessage.Key))
            {
                throw new AuthenticationException("Broadcasted node beacon doesn't correspond to signature public key.");
            }
        }

        private DKGRequest CreateDKGMessage(string proverNodeId)
        {
            var dkgRequest = new DKGRequest
            {
                NodeId = proverNodeId
            };

            authentication.Sign(dkgRequest);
            return dkgRequest;
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