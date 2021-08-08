using System.Collections.Generic;
using System.Threading.Tasks;
using Theseus.Core;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Theseus.Core.Messages;
using Theseus.Core.Tests;
using Xunit;

namespace Theseus.Demo.Tests
{
    public class AdHocSrwcServiceTests
    {
        private readonly FakeWorld _fakeWorld;

        public AdHocSrwcServiceTests()
        {
            _fakeWorld = new FakeWorld();
        }

        [Fact]
        public async Task SubsetOfColleguesIsInRange_MessageDeliveredOnlyToThem()
        {
            //Given
            var service = new AdHocSrwcService(considerCoordinates: true);

            var sourceSignalCoords = 1;
            var inRangeCoords = 3;
            var notInRangeCoords = 6;

            var sourceNode = CreateNodeWithLocation(service, sourceSignalCoords);
            var inRangeNode = CreateNodeWithLocation(service, inRangeCoords);
            var notInRangeNode = CreateNodeWithLocation(service, notInRangeCoords);

            service.RegisterColleagues(new List<FakeNodeGateway> {
                sourceNode, inRangeNode, notInRangeNode });

            //When
            await sourceNode.Node.BroadcastPersonalBeacon();

            //Then
            Assert.False(((FakeReceivingNode)notInRangeNode.Node).Received);
            Assert.True(((FakeReceivingNode)inRangeNode.Node).Received);
        }

        private FakeNodeGateway CreateNodeWithLocation(AdHocSrwcService service, int sourceSignalCoords)
        {
            var coordinates = new Coordinates { X = sourceSignalCoords };
            var fakeGps = new FakeGPS(coordinates);
            var fakeNavigator = new FakeNavigator(fakeGps, _fakeWorld.Ticker);
            var node = new FakeReceivingNode(service, Common.CreateAuth(), fakeGps, null, null, null, fakeNavigator);
            return new FakeNodeGateway(node);
        }

        internal class FakeReceivingNode : Node
        {

            public bool Received = false;

            public FakeReceivingNode(
                ISrwcService srwcService,
                IAuthentication authentication,
                IGPS gps,
                IWANCommunication wanCommunication,
                IDKGClient dkgClient,
                IMessageLog messageLog,
                INavigation navigation) : base(
                    srwcService,
                    authentication,
                    gps,
                    wanCommunication,
                    dkgClient,
                    messageLog,
                    navigation, 
                    Common.CreateStubState())
            {
            }

            public override void ReceiveBeacon(Beacon beaconMessage)
            {
                Received = true;
            }
        }
    }
}