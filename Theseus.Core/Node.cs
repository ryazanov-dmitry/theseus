using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theseus.Core.Classificators;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Theseus.Core.Exceptions;
using Theseus.Core.Messages;
using Theseus.Core.Navigation;
using Theseus.Core.States;

namespace Theseus.Core
{
    public interface INode : IMovable
    {
        string Id { get; }
        IDKGClient DKGClient { get; }

        Task BroadcastPersonalBeacon();
        List<string> GetDKGPubs();
        void ReceiveBeacon(Beacon beaconMessage);
        Task ReceiveDKG(DKGRequest message);
        Task RequestDKG(string proverNodeId, float proverCoords);
        Task ReceiveDeliveryRequest(DeliveryRequest deliveryRequest);
        void ReceiveDKGPublicKey(DKGPub DKGPub);
        Coordinates GetCoordinates();
    }

    public class Node : INode
    {
        public string Id => authentication.Base64PublicKey();

        // TODO: I am not sure we want it public..

        public IDKGClient DKGClient => dkgClient;

        public const int BeaconValidTime = 5;
        private const int dkgSessionTimeout = 1;
        private const int _signalRange = 3;
        private readonly ISrwcService srwcService;
        private readonly IAuthentication authentication;
        private readonly IGPS gps;
        private readonly Coordinates coordinates;
        private readonly IWANCommunication wanCommunication;

        private readonly IDKGClient dkgClient;
        private readonly IMessageLog messageLog;
        private readonly INavigation navigation;
        private readonly NodeType nodeType;
        private readonly IRequestAndStateValidator requestAndStateValidator;
        private Coordinates currentClientCoords;
        private readonly HashSet<object> lastDkgRequests = new HashSet<object>();


        public Node(
            ISrwcService srwcService,
            IAuthentication authentication,
            IGPS gps,
            IWANCommunication wanCommunication,
            IDKGClient dkgClient,
            IMessageLog messageLog,
            INavigation navigation,
            IRequestAndStateValidator requestAndStateValidator)
        {
            this.authentication = authentication;
            this.srwcService = srwcService;
            this.gps = gps;
            this.coordinates = gps.GetGPSCoords();
            this.wanCommunication = wanCommunication;
            this.dkgClient = dkgClient;
            this.messageLog = messageLog;
            this.navigation = navigation;
            this.requestAndStateValidator = requestAndStateValidator;
        }

        public Node(
            ISrwcService srwcService,
            IGPS gps,
            IWANCommunication wanCommunication,
            IDKGClient dkgClient,
            IMessageLog messageLog,
            INavigation navigation,
            NodeType nodeType)
        {
            this.authentication = new Authentication(new RSA());
            this.srwcService = srwcService;
            this.gps = gps;
            this.coordinates = gps.GetGPSCoords();
            this.wanCommunication = wanCommunication;
            this.dkgClient = dkgClient;
            this.messageLog = messageLog;
            this.navigation = navigation;
            this.nodeType = nodeType;
            this.requestAndStateValidator = new RequestAndStateValidator(nodeType);
        }


        public async Task BroadcastPersonalBeacon()
        {
            var message = CreateBeaconMessage();
            await srwcService.Broadcast(message, this);
        }

        public async Task RequestDKG(string proverNodeId, float proverCoords)
        {
            var message = CreateDKGMessage(proverNodeId, proverCoords);
            await srwcService.Broadcast(message, this);
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

            if (ExistsDKGSessionForThisBeacon(beaconMessage))
            {
                var dkgSessionId = GetCurrentDkgSessionId(beaconMessage.Id);
                dkgClient.SendAccept(dkgSessionId.Value);
            }
        }

        public async Task ReceiveDKG(DKGRequest dKGRequest)
        {
            authentication.Verify(dKGRequest);

            if (TheSame(dKGRequest))
                return;

            if (requestAndStateValidator.IsNotValid(dKGRequest))
                return;

            if (Geometry.Distance(dKGRequest.GPSCoordinates, coordinates.X) > _signalRange)
            {
                await srwcService.Broadcast(dKGRequest, this);
                return;
            }

            if (!ExistsValidBeacon(dKGRequest))
            {
                wanCommunication.SendWarning();
                return;
            }

            await dkgClient.TryInitDKGSession(dKGRequest.NodeId);

            requestAndStateValidator.Transition(States.States.Verifier.InitedDkgSession);
        }

        private bool TheSame(DKGRequest dKGRequest)
        {
            if (lastDkgRequests.Contains(dKGRequest))
            {
                return true;
            }

            lastDkgRequests.Add(dKGRequest);
            return false;
        }

        private void RegisterReceivedBeacon(Beacon beaconMessage)
        {
            messageLog.Log(beaconMessage.Base64Key(), beaconMessage);
        }

        private void CheckPayloadNodeIdAndPublicKey(Beacon beaconMessage)
        {
            if (beaconMessage.Id != System.Convert.ToBase64String(beaconMessage.Key))
            {
                throw new AuthenticationException("Broadcasted node beacon doesn't correspond to signature public key.");
            }
        }

        private DKGRequest CreateDKGMessage(string proverNodeId, float proverCoords)
        {
            var dkgRequest = new DKGRequest
            {
                NodeId = proverNodeId,
                GPSCoordinates = proverCoords
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
            return messageLog
                .Get<Beacon>(sender: dKGRequest.NodeId, lastMinutes: BeaconValidTime)
                .Any();
        }

        private bool ExistsDKGSessionForThisBeacon(Beacon beaconMessage)
        {
            return GetCurrentDkgSessionId(beaconMessage.Id) != null;
        }

        private Guid? GetCurrentDkgSessionId(string beaconProverId)
        {
            return messageLog
                .Get<DKGInitRequest>(dkgSessionTimeout)
                .Where(x => x.NodeId == beaconProverId)
                .LastOrDefault()?.SessionId;
        }

        public async Task ReceiveDeliveryRequest(DeliveryRequest deliveryRequest)
        {
            // TODO: Validate

            requestAndStateValidator.Transition(deliveryRequest);

            // Send DKGRequest
            currentClientCoords = new Coordinates { X = deliveryRequest.GPSCoordinates };
            await RequestDKG(deliveryRequest.NodeId, deliveryRequest.GPSCoordinates);
        }

        // TODO: we need to receive multiple Public keys
        public void ReceiveDKGPublicKey(DKGPub DKGPub)
        {
            // TODO: Validate

            // TODO: Persist current session

            NavigateToClient(currentClientCoords);
        }

        public void ReceiveContract(DeliveryContract DKGPub)
        {
            // TODO: Validate

            // TODO: Persist current session

            NavigateToClient(currentClientCoords);
        }

        private void NavigateToClient(Coordinates currentClientCoords)
        {
            navigation.NavigateTo(currentClientCoords);
        }

        public bool IsMoving()
        {
            return navigation.IsMoving();
        }

        public Coordinates GetCoordinates()
        {
            return this.coordinates;
        }
    }
}