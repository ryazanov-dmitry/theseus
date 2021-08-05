using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Theseus.Core;
using Theseus.Core.Classificators;
using Theseus.Core.Dto;
using Theseus.Core.Messages;
using Theseus.Core.Tests;
using Xunit;

namespace Theseus.Demo.Tests
{
    public class SignalPropagationTests
    {
        /// <summary>
        /// I want to test if requester can send DkgRequest and proper group of 
        /// nodes with client will start DKG 
        /// </summary>
        [Fact]
        public async Task TestName()
        {
            //Given
            var world = new FakeWorld();
            var medium = new AdHocSrwcService(
                considerCoordinates: true, writeToSeparateConsole: true);
            var messageLog = new MessageLog(true);

            var targetVerfierDkgClientMock = new Mock<IDKGClient>();

            var requesterNode = CreateNode(world, medium, 1, messageLog, NodeType.Service);
            var verifier1 = CreateNode(world, medium, 3, messageLog, NodeType.Verifier);
            var verifier2 = CreateNode(world, medium, 5, messageLog, NodeType.Verifier);
            var targetVerfier = CreateNode(world, medium, 8, messageLog, NodeType.Verifier, targetVerfierDkgClientMock.Object);
            var client = CreateNode(world, medium, 9, messageLog, NodeType.Client);

            targetVerfierDkgClientMock.Setup(x => x.TryInitDKGSession(client.Node.Id));

            medium.RegisterColleagues(new List<FakeNodeGateway> {
                requesterNode, verifier1, verifier2, targetVerfier, client });

            var deliveryRequest = new DeliveryRequest
            {
                GPSCoordinates = 9,
                NodeId = client.Node.Id
            };

            //When
            await client.Node.BroadcastPersonalBeacon();
            await requesterNode.Node.ReceiveDeliveryRequest(deliveryRequest);

            //Then
            targetVerfierDkgClientMock.Verify(x => x.TryInitDKGSession(client.Node.Id), Times.Once);
        }

        private static FakeNodeGateway CreateNode(
            FakeWorld world, 
            AdHocSrwcService medium, 
            int coord, 
            MessageLog messageLog,
            NodeType nodeType,
            IDKGClient dkgClient = null)
        {
            var requesterGps = new FakeGPS(new Coordinates { X = coord });

            var requesterNode = new Node(
                medium, requesterGps, null, dkgClient, messageLog, new FakeNavigator(requesterGps, world.Ticker), nodeType
            );

            return new FakeNodeGateway(requesterNode);
        }
    }
}