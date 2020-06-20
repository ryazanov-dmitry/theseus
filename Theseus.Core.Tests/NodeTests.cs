using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
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
            srwcServiceMock.Verify(x => x.Broadcast("node 1 beacon"));            
        }
    }
}
