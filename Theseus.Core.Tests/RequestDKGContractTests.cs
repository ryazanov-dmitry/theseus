using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Theseus.Core.Infrastructure;
using Theseus.Core.Messages;
using Xunit;

namespace Theseus.Core.Tests
{
    public class RequestDKGContractTests
    {
        private const int defaultCoords = 1;
        private readonly AdHocSrwcService srwcService;
        private Mock<IAuthentication> authenticationMock;
        private IAuthentication authentication;

        private readonly Mock<IGPS> gps;
        private readonly Mock<IWANCommunication> wanCommunication;
        private readonly Mock<IDKGClient> dkgClient;

        public RequestDKGContractTests()
        {
            srwcService = CreateSrwc();
            authenticationMock = new Mock<IAuthentication>();
            authentication = authenticationMock.Object;
            gps = CreateGPSMock();
            wanCommunication = new Mock<IWANCommunication>();
            dkgClient = CreateDKGClientMock();
        }

        [Fact]
        public async Task RequestDKG_4LoyalDKGNodes_RequestorReceives1DKGPub()
        {
            //Arrange
            var requestor = CreateNode();
            var prover = CreateNode();
            var dkgNode1 = CreateNode();
            var dkgNode2 = CreateNode();
            var dkgNode3 = CreateNode();
            var dkgNode4 = CreateNode();

            srwcService.RegisterColleagues(new List<FakeNodeGateway> { requestor, prover, dkgNode1, dkgNode2, dkgNode3, dkgNode4 });

            //Act
            await prover.Node.BroadcastPersonalBeacon();
            await requestor.Node.RequestDKG(prover.Node.Id, defaultCoords);

            //Assert
            dkgClient.Verify(x => x.TryInitDKGSession(It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public async Task RequestDKG_ClaimedGPSDiffersFromNodesLocation_IgnoreAndPropagateRequest()
        {
            //Arrange
            var srwcMock = new Mock<ISrwcService>();
            srwcMock.Setup(x => x.Broadcast(It.IsAny<object>(), It.IsAny<object>()));
            var receiverNode = CreateNode(srwcMock.Object);

            var dkgRequest = new DKGRequest
            {
                GPSCoordinates = 5
            };
            Common.CreateAuth().Sign(dkgRequest);

            //Act
            await receiverNode.Receive(dkgRequest);

            //Assert
            srwcMock.Verify(x => x.Broadcast(It.Is<DKGRequest>(x => x.Equals(dkgRequest)), It.IsAny<object>()),
                Times.Once);

        }

        [Fact]
        public async Task RequestDKG_NoBeaconReceivedWithClaimedId_SendWarning()
        {
            //Arrange
            var receiverNode = CreateNode();
            var dkgRequest = new DKGRequest
            {
                GPSCoordinates = defaultCoords
            };
            Common.CreateAuth().Sign(dkgRequest);

            wanCommunication.Setup(x => x.SendWarning());

            //Act
            await receiverNode.Receive(dkgRequest);

            //Assert
            wanCommunication.Verify(x => x.SendWarning(), Times.Once);
        }

        private FakeNodeGateway CreateNode()
        {
            return CreateNode(srwcService);
        }

        private FakeNodeGateway CreateNode(ISrwcService srwcService)
        {
            var node = new Node(
                srwcService,
                Common.CreateAuth(), 
                gps.Object, 
                wanCommunication.Object, 
                dkgClient.Object, 
                new MessageLog(), 
                null, 
                Common.CreateStubState());

            return new FakeNodeGateway(node);
        }

        private AdHocSrwcService CreateSrwc()
        {
            return new AdHocSrwcService();
        }
        private Mock<IGPS> CreateGPSMock()
        {
            var mock = new Mock<IGPS>();
            mock.Setup(x => x.GetGPSCoords()).Returns(new Coordinates
            {
                X = defaultCoords
            });
            return mock;
        }

        private Mock<IDKGClient> CreateDKGClientMock()
        {
            var mock = new Mock<IDKGClient>();
            mock.Setup(x => x.TryInitDKGSession(It.IsAny<string>()));
            return mock;
        }
    }
}