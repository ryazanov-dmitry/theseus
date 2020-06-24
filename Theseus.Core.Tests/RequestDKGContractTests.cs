using System.Threading.Tasks;
using Moq;
using Theseus.Core.Crypto;
using Xunit;

namespace Theseus.Core.Tests
{
    public class RequestDKGContractTests
    {
        private readonly AdHocSrwcService srwcService;
        private IAuthentication authentication;
        private Mock<IAuthentication> authenticationMock;

        public RequestDKGContractTests()
        {
            srwcService = CreateSrwc();
            authenticationMock = new Mock<IAuthentication>();
            authentication = authenticationMock.Object;
        }

        [Fact]
        public async Task RequestDKG_4LoyalDKGNodes_RequestorReceives1DKGPub()
        {
            //Arrange
            var srwcService = CreateSrwc();
            var requestor = CreateNode();
            var prover = CreateNode();
            var dkgNode1 = CreateNode();
            var dkgNode2 = CreateNode();
            var dkgNode3 = CreateNode();
            var dkgNode4 = CreateNode();

            // srwcService.RegisterColleagues(new List)

            //Act
            await requestor.RequestDKG(prover.Id);

            //Assert
            var dkgPubs = requestor.GetDKGPubs();
            Assert.NotEmpty(dkgPubs);
        }

        private Node CreateNode()
        {
            return new Node(srwcService, authentication);
        }

        private AdHocSrwcService CreateSrwc()
        {
            return new AdHocSrwcService();
        }
    }
}