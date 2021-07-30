using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Theseus.Core;
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


            var requesterNode = CreateNode(world, medium, 1, messageLog);
            var verifier1 = CreateNode(world, medium, 3, messageLog);
            var verifier2 = CreateNode(world, medium, 5, messageLog);
            var targetVerfier = CreateNode(world, medium, 8, messageLog);
            var client = CreateNode(world, medium, 9, messageLog);

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
        }

        private static FakeNodeGateway CreateNode(FakeWorld world, AdHocSrwcService medium, int coord, MessageLog messageLog)
        {
            var requesterGps = new FakeGPS(new Coordinates { X = coord });

            var dkgClientMock = new Mock<IDKGClient>();
            var requesterNode = new Node(
                medium, requesterGps, null, dkgClientMock.Object, messageLog, new FakeNavigator(requesterGps, world.Ticker)
            );

            return new FakeNodeGateway(requesterNode);
        }
    }
}