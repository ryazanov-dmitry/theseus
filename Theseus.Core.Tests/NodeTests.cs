using System;
using System.Threading.Tasks;
using Moq;
using Theseus.Core.Crypto;
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
            var auth = new Mock<IAuthentication>();
            var node = new Node(srwcServiceMock.Object, auth.Object);

            //Act
            await node.BroadcastPersonalBeacon();

            //Assert
            srwcServiceMock.Verify(x => x.Broadcast(It.IsAny<Beacon>()));
        }

        /// <summary>
        /// Nodes sign theirs beacons, so that other subjects cannot act as other loyal nodes.
        /// </summary>
        [Fact]
        public void ReceiveBeacon_SignedWithWrongKey_Throws()
        {
            //Arrange
            var srwcServiceMock = new Mock<ISrwcService>();
            var rsa = new RSA();
            var auth = new Authentication(rsa);
            var node = new Node(srwcServiceMock.Object, auth);
            var beaconMessage = new Beacon
            {
                Id = Guid.NewGuid().ToString()
            };
            beaconMessage.Signature = rsa.HashAndSign(
                beaconMessage.PlainData(), rsa.GenerateKeyPair());

            beaconMessage.Key = rsa.GenerateKeyPair().Modulus;

            //Act, Assert
            Assert.Throws<AuthenticationException>(() => node.ReceiveBeacon(beaconMessage));
        }

        [Fact]
        public void ReceiveBeacon_CorrectSignature_Returns()
        {
            //Arrange
            var srwcServiceMock = new Mock<ISrwcService>();
            var rsa = new RSA();
            var auth = new Authentication(rsa);
            var node = new Node(srwcServiceMock.Object, auth);
            var key = rsa.GenerateKeyPair();
            var beaconMessage = new Beacon
            {
                Id = Guid.NewGuid().ToString()
            };
            beaconMessage.Signature = rsa.HashAndSign(
                beaconMessage.PlainData(), key);

            beaconMessage.Key = key.Modulus;

            //Act
            node.ReceiveBeacon(beaconMessage);
        }

        [Fact]
        public void ReceiveDKGRequest_IncorrectSignature_Throws()
        {
            // var node = new Node(new Mock<ISrwcService>().Object);
            // var dKGRequest = new DKGRequest{};

            // node.ReceiveDKGrequest(DKGRequest);
        }
    }
}
