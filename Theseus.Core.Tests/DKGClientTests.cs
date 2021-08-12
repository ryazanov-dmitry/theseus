using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Theseus.Core.Infrastructure;
using Theseus.Core.Messages;
using Theseus.Core.Tests.Factories;
using Xunit;
using Xunit.Abstractions;

namespace Theseus.Core.Tests
{
    public class DKGClientTests
    {
        private readonly Mock<IAuthentication> auth;
        private readonly ITestOutputHelper o;
        private AdHocSrwcService medium;

        public DKGClientTests(ITestOutputHelper o)
        {
            this.o = o;
            auth = new Mock<IAuthentication>();
            medium = new AdHocSrwcService(o);
        }

        [Fact]
        public async Task CreateDKGInitRequest_NoSessionFirstInGroupToTriggerDKG_ReceiveAcceptFromOtherNode()
        {
            //Arrange
            var initiatorOfDKG = CreateClient();

            const string proverId = "someNodeId";

            var dkgAcceptorLog = new MessageLog();
            dkgAcceptorLog.Log(proverId, new Beacon());

            var dkgAcceptor = CreateClient(dkgAcceptorLog);
            medium.RegisterColleagues(new List<FakeNodeGateway> { initiatorOfDKG, dkgAcceptor });

            //Act
            await initiatorOfDKG.Node.DKGClient.TryInitDKGSession(proverId);

            //Assert
            Assert.True(medium.GetLastMessageFrom(dkgAcceptor.Node.DKGClient) is DKGStartSessionAccept lastMessage);
        }


        [Fact]
        public void NodeReceivesBeaconAfterDKGSessionStart_NodeSendsTryInitDKGSession_MasterNodeResponds()
        {
            //Arrange
            var proverAuth = Common.CreateAuth();
            var proverId = proverAuth.Base64PublicKey();
            var proverBeacon = new Beacon
            {
                Id = proverId
            };
            proverAuth.Sign(proverBeacon);

            var masterNodeAuth = Common.CreateAuth();
            var masterNode = CreateClient(new MessageLog(), masterNodeAuth);

            var lateAuth = Common.CreateAuth();
            var lateNodeMessageLog = new MessageLog();
            lateNodeMessageLog.Log(masterNodeAuth.Base64PublicKey(), new DKGInitRequest
            {
                NodeId = proverId
            });
            var lateDkgClient = CreateClient(lateNodeMessageLog);
            var lateNode = CreateNode(medium, lateAuth, lateDkgClient.Node.DKGClient, lateNodeMessageLog);

            //Act
            lateNode.ReceiveBeacon(proverBeacon);

            //Assert
            Assert.True(medium.GetLastMessageFrom(lateDkgClient.Node.DKGClient) is DKGStartSessionAccept lastMessage);
        }


        /// <summary>
        /// Lets assume we can use central time synchronization service. Node will store send time in request.
        /// When node receives request it checks who was first and acts accordingly. For that node needs to persists its request time.
        /// </summary>
        [Fact]
        public async Task TryInitDKGSession_2NodesSendDKGStartSimultaneously_2NodeAccepts()
        {
            //Arrange
            const string proverNodeId = "proverNode";
            var node1MessageLog = new MessageLog();
            node1MessageLog.Log(proverNodeId, new Beacon());

            var node2MessageLog = new MessageLog();
            node2MessageLog.Log(proverNodeId, new Beacon());
            var node2Auth = Common.CreateAuth();
            node2MessageLog.Log(
                node2Auth.Base64PublicKey(),
                new DKGInitRequest
                {
                    NodeId = proverNodeId,
                    SentTime = DateTime.Now.AddMinutes(1)
                });


            var node1 = CreateClient(node1MessageLog);
            var node2 = CreateClient(node2MessageLog, node2Auth);

            medium.RegisterColleagues(new List<FakeNodeGateway> { node1, node2 });

            //Act
            await node1.Node.DKGClient.TryInitDKGSession(proverNodeId);

            //Assert
            Assert.Equal(2, medium.GetAllMessages().Count);
            Assert.True(medium.GetLastMessageFrom(node2.Node.DKGClient) is DKGStartSessionAccept);

            Thread.Sleep(3000);
            Assert.True(medium.GetLastMessageFrom(node1.Node.DKGClient) is DKGSessionList);
        }

        [Fact]
        public async Task ReceiveTryInitDKGSession_Before_TryInitDKGSession()
        {
            //Given
            const string clientNodeId = "proverNode";

            var firstVerifier = CreateClient();
            var secondVerifier = CreateNodeWithBeacon(clientNodeId);

            medium.RegisterColleagues(new List<FakeNodeGateway> {
                firstVerifier,
                secondVerifier});

            //When
            await firstVerifier.Node.DKGClient.TryInitDKGSession(clientNodeId);

            //Then
            var lastMessage = medium.GetLastMessageFrom(secondVerifier.Node.DKGClient);
            Assert.True(lastMessage is DKGStartSessionAccept);
        }

        private FakeNodeGateway CreateNodeWithBeacon(string clientNodeId)
        {
            var node1MessageLog = new MessageLog();
            node1MessageLog.Log(clientNodeId, new Beacon());

            return CreateClient(node1MessageLog);
        }

        [Fact(Skip = "future")]
        public void ReceiveTryInitDKGSession_NotReadyForDKG_SendDecline()
        {

        }

        [Fact(Skip = "future")]
        public void MoreThan1ParallelDKGSessionStart()
        {

        }


        /// <summary>
        /// If Node receives session list:
        /// 1. We assume that all nodes receive the same list
        /// 2. Node checks if its Id is in the list.
        /// 3. Node sends contract to client and service with WAN
        ///     3.1 Contract includes:
        ///         - node's public key
        ///         - client id
        ///         - geolocation
        /// </summary>
        [Fact]
        public async Task ReceiveDKGSessionList_IdInTheList_SendsPub()
        {
            //Given
            var serviceId = "serviceId";
            var clientId = "clientId";

            var wanComm = new Mock<IDKGCommunicator>();
            wanComm.Setup(x => x.SendDKGPub(It.IsAny<string>(), serviceId));
            wanComm.Setup(x => x.SendDKGPub(It.IsAny<string>(), clientId));

            var verifierNode = DKGClientFactory.CreateDkgClient(wanComm.Object);

            var sessionId = Guid.NewGuid();
            var list = new DKGSessionList
            {
                Participants = new List<string>{},
                SessionId = sessionId
            };

            //When
            await verifierNode.ReceiveDKGSessionList(list);

            //Then
            wanComm.Verify(x => x.SendDKGPub(It.IsAny<string>(), serviceId), Times.Once);
            wanComm.Verify(x => x.SendDKGPub(It.IsAny<string>(), clientId), Times.Once);

        }

        private FakeNodeGateway CreateClient()
        {
            return CreateClient(new MessageLog(), Common.CreateAuth());
        }

        private FakeNodeGateway CreateClient(MessageLog messageLog)
        {
            return CreateClient(messageLog, Common.CreateAuth());
        }


        private FakeNodeGateway CreateClient(MessageLog messageLog, IAuthentication auth)
        {
            var gpsMock = new Mock<IGPS>();

            var client = new DKGClient(medium, auth, messageLog, new Session(), new Chronos());
            var node = new Node(medium, auth, gpsMock.Object, null, client, messageLog, null, Common.CreateStubState());
            var gateway = new FakeNodeGateway(node);
            return gateway;
        }

        private Node CreateNode(ISrwcService srwcService, IAuthentication auth, IDKGClient dKGClient, MessageLog log)
        {
            var gpsMock = new Mock<IGPS>();
            var wanComm = new Mock<IWANCommunication>();

            return new Node(srwcService, auth, gpsMock.Object, wanComm.Object, dKGClient, log, null, Common.CreateStubState());
        }
    }
}