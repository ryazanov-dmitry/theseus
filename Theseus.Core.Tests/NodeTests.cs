using System;
using System.Threading.Tasks;
using Moq;
using Theseus.Core.Dto;
using Theseus.Core.Exceptions;
using Xunit;

namespace Theseus.Core.Tests
{
    public class NodeTests
    {
        [Fact]
        public async Task BroadcastPersonalBeacon_HappyPath()
        {
            //Arrange
            var srwcServiceMock = new Mock<ISrwcService>();
            var node = new Node(srwcServiceMock.Object);

            //Act
            await node.BroadcastPersonalBeacon();

            //Assert
            srwcServiceMock.Verify(x => x.Broadcast(It.IsAny<Beacon>()));
        }

        /// <summary>
        /// Nodes sign theirs beacons, so that other subjects cannot act as other loyal nodes.
        /// </summary>
        [Fact]
        public void ReceiveBeacon_IncorrectSignature_Throws()
        {
            //Arrange
            var srwcServiceMock = new Mock<ISrwcService>();
            var node = new Node(srwcServiceMock.Object);
            var beaconMessage = new Beacon
            {
                Id = Guid.NewGuid().ToString()
            };

            //Act, Assert
            Assert.Throws<AuthenticationException>(() => node.ReceiveBeacon(beaconMessage));
        }

        [Fact]
        public void ReceiveDKGRequest_IncorrectSignature_Throws()
        {
            var node = new Node(new Mock<ISrwcService>().Object);
            var DKGrequest = new DKGRequest{};

            // node.ReceiveDKGrequest(DKGRequest);
        }
    }
}
