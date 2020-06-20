using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Theseus.Core.Tests
{
    public class ProtocolUseCasesTests
    {
        [Fact]
        public async Task NodeCanBroadcastHimself()
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
