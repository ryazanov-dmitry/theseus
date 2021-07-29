using Moq;
using Theseus.Core;
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
        public void TestName()
        {
            //Given
            var world = new FakeWorld();
            var medium = new AdHocSrwcService(considerCoordinates: true);

            var requesterGps = new FakeGPS(new Coordinates { X = 1 });

            var dkgClientMock = new Mock<IDKGClient>();
            var messageLog = new MessageLog();
            var requesterNode = new Node(
                medium, requesterGps, null, dkgClientMock.Object, messageLog, new FakeNavigator(requesterGps, world.Ticker)
            );

            //When

            //Then
        }
    }
}