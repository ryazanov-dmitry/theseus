using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Theseus.Core.Crypto;
using Xunit;

namespace Theseus.Core.Tests
{
    public class RequestDKGContractTests
    {
        private readonly AdHocSrwcService _srwcService;
        private IAuthentication authentication;
        private Mock<IAuthentication> authenticationMock;

        public RequestDKGContractTests()
        {
            _srwcService = CreateSrwc();
            authenticationMock = new Mock<IAuthentication>();
            authentication = authenticationMock.Object;
        }

        [Fact]
        public async Task RequestDKG_4LoyalDKGNodes_RequestorReceives1DKGPub()
        {
            //Arrange
            var requestor = CreateNode();
            var prover = CreateNode();
            var dkgNode1 = CreateNode();
            var dkgNode2 = CreateNode();
            var dkgNode3 = CreateNode();
            var dkgNode4 = CreateNode();

            _srwcService.RegisterColleagues(new List<INode>{requestor,prover,dkgNode1,dkgNode2,dkgNode3, dkgNode4});

            //Act
            await prover.BroadcastPersonalBeacon();
            await requestor.RequestDKG(prover.Id);

            //Assert
            var dkgPubs = requestor.GetDKGPubs();
            Assert.NotEmpty(dkgPubs);
        }

        private Node CreateNode()
        {
            return new Node(_srwcService, authentication);
        }

        private AdHocSrwcService CreateSrwc()
        {
            return new AdHocSrwcService();
        }
    }
}