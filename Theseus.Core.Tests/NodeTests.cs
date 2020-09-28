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
        private readonly Mock<ISrwcService> srwcServiceMock;
        private readonly Mock<IAuthentication> authentication;

        private readonly Mock<IGPS> gps;
        private readonly Mock<IWANCommunication> wanCommunication;

        public NodeTests()
        {
            srwcServiceMock = new Mock<ISrwcService>();
            authentication = new Mock<IAuthentication>();
            gps = new Mock<IGPS>();
            wanCommunication = new Mock<IWANCommunication>();
        }

        [Fact]
        public async Task BroadcastPersonalBeacon_HappyPath()
        {
            //Arrange
            var node = CreateNode();

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
            var rsa = new RSA();
            var auth = new Authentication(rsa);
            var node = CreateNode(auth);
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
            var gps = new Mock<IGPS>();
            var rsa = new RSA();
            var auth = new Authentication(rsa);
            var node = CreateNode(auth);
            var key = rsa.GenerateKeyPair();
            var beaconMessage = new Beacon
            {
                Id = System.Convert.ToBase64String(key.Modulus)
            };
            beaconMessage.Signature = rsa.HashAndSign(
                beaconMessage.PlainData(), key);

            beaconMessage.Key = key.Modulus;

            //Act
            node.ReceiveBeacon(beaconMessage);
        }

        [Fact]
        public void ReceiveBeacon_PayloadNodeIdDiffersFromPublicKey_Throws()
        {
            //Arrange
            var rsa = new RSA();
            var auth = new Authentication(rsa);
            var node = CreateNode(auth);
            var key = rsa.GenerateKeyPair();
            var beaconMessage = new Beacon
            {
                Id = System.Convert.ToBase64String(rsa.GenerateKeyPair().Modulus)
            };
            beaconMessage.Signature = rsa.HashAndSign(
                beaconMessage.PlainData(), key);

            beaconMessage.Key = key.Modulus;

            //Act
            var exception = Assert.Throws<AuthenticationException>(() => node.ReceiveBeacon(beaconMessage));

            //Assert
            Assert.Equal("Broadcasted node beacon doesn't correspond to signature public key.", exception.Message);

        }

        // [Fact]
        // public void ReceiveDKGRequest_IncorrectSignature_Throws()
        // {
        //     Assert.True(false);
        // }
        
        private Node CreateNode(IAuthentication auth)
        {
            return new Node(srwcServiceMock.Object, auth, gps.Object, wanCommunication.Object);
        }

        private Node CreateNode()
        {
            return CreateNode(authentication.Object);
        }
    }
}
