using Theseus.Core;
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
        public void SubsetOfColleguesIsInRange_MessageDeliveredOnlyToThem()
        {
            //Given
            var service = new AdHocSrwcService();

            var sourceSignalCoords = 1;
            var inRangeCoords = 3;
            var notInRangeCoords = 6;

            var sourceNode = CreateNodeWithLocation(service, sourceSignalCoords);
            var inRangeNode = CreateNodeWithLocation(service, inRangeCoords);
            var notInRangeNode = CreateNodeWithLocation(service, notInRangeCoords);

            //When
            sourceNode.

            //Then
        }

        private FakeNodeGateway CreateNodeWithLocation(AdHocSrwcService service, int sourceSignalCoords)
        {
            var coordinates = new Coordinates { X = sourceSignalCoords };
            var fakeGps = new FakeGPS(coordinates);
            var fakeNavigator = new FakeNavigator(fakeGps, coordinates, _fakeWorld.Ticker);
            var node = new Node(service, Common.CreateAuth(), fakeGps, null, null, null, fakeNavigator);
            return new FakeNodeGateway(node);
        }
    }
}