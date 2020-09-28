using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IGPS gps;
        private readonly Coordinates coordinates;
        private readonly List<ReceivedBeacon> receivedBeacons;
        private readonly IWANCommunication wanCommunication;

        public Node(
            ISrwcService srwcService,
            IAuthentication authentication,
            IGPS gps,
            IWANCommunication wanCommunication)
        {
            authentication.SetupRSAKeyPairs();
            this.authentication = authentication;
            this.srwcService = srwcService;
            this.receivedBeacons = new List<ReceivedBeacon>();
            this.gps = gps;
            this.coordinates = gps.GetGPSCoords();
            this.wanCommunication = wanCommunication;
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
            RegisterReceivedBeacon(beaconMessage);
        }

        public Task ReceiveDKG(DKGRequest dKGRequest)
        {
            authentication.Verify(dKGRequest);

            if (!dKGRequest.GPSCoordinates.Equals(coordinates.DummyCoords))
            {
                srwcService.Broadcast(dKGRequest);
                return null; //TODO: Is it ok?
            }

            if(!ExistsValidBeacon(dKGRequest)){
                wanCommunication.SendWarning();
                return null;
            }

            //start dkg generation
            throw new NotImplementedException();
        }


        private void RegisterReceivedBeacon(Beacon beaconMessage)
        {
            var receivedBeacon = RegisterReceiveTime(beaconMessage);
            receivedBeacons.Add(receivedBeacon);
        }

        private ReceivedBeacon RegisterReceiveTime(Beacon beaconMessage)
        {
            return new ReceivedBeacon
            {
                Id = beaconMessage.Id,
                ReceivedDateTime = DateTime.Now
            };
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
            var beacon = new Beacon
            {
                Id = Id
            };
            authentication.Sign(beacon);
            return beacon;
        }

        private bool ExistsValidBeacon(DKGRequest dKGRequest)
        {
            return receivedBeacons
                .Where(x => x.ReceivedDateTime > DateTime.Now.AddMinutes(5))
                .FirstOrDefault(x => x.Id == dKGRequest.NodeId) != null;
        }
    }
}