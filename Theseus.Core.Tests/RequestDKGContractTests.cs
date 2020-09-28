using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Xunit;

namespace Theseus.Core.Tests
{
    public class RequestDKGContractTests
    {
        private const int defaultCoords = 1;
        private readonly AdHocSrwcService srwcService;
        private Mock<IAuthentication> authenticationMock;
        private readonly Mock<IGPS> gps;
        private readonly Mock<IWANCommunication> wanCommunication;

        public RequestDKGContractTests()
        {
            srwcService = CreateSrwc();
            authenticationMock = new Mock<IAuthentication>();
            gps = CreateGPSMock();
            wanCommunication = new Mock<IWANCommunication>();

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

            srwcService.RegisterColleagues(new List<INode> { requestor, prover, dkgNode1, dkgNode2, dkgNode3, dkgNode4 });

            //Act
            await prover.BroadcastPersonalBeacon();
            await requestor.RequestDKG(prover.Id);

            //Assert
            var dkgPubs = requestor.GetDKGPubs();
            Assert.NotEmpty(dkgPubs);
        }

        [Fact]
        public void RequestDKG_ClaimedGPSDiffersFromNodesLocation_IgnoreAndPropagateRequest()
        {
            //Arrange
            var srwcMock = new Mock<ISrwcService>();
            srwcMock.Setup(x => x.Broadcast(It.IsAny<object>()));
            var receiverNode = CreateNode(srwcMock.Object);
            var dkgRequest = new DKGRequest
            {
                GPSCoordinates = 2
            };

            //Act
            receiverNode.ReceiveDKG(dkgRequest);

            //Assert
            srwcMock.Verify(x => x.Broadcast(It.Is<DKGRequest>(x => x.Equals(dkgRequest))),
                Times.Once);

        }

        [Fact]
        public void RequestDKG_NoBeaconReceivedWithClaimedId_SendWarning()
        {
            //Arrange
            var receiverNode = CreateNode();
            var dkgRequest = new DKGRequest
            {
                GPSCoordinates = defaultCoords
            };
            wanCommunication.Setup(x => x.SendWarning());

            //Act
            receiverNode.ReceiveDKG(dkgRequest);

            //Assert
            wanCommunication.Verify(x => x.SendWarning(), Times.Once);
        }

        private Node CreateNode()
        {
            return CreateNode(srwcService);
        }

        private Node CreateNode(ISrwcService srwcService)
        {
            return new Node(srwcService, authenticationMock.Object, gps.Object, wanCommunication.Object);
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
                DummyCoords = defaultCoords
            });
            return mock;
        }
    }
}