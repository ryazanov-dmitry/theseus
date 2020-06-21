using System.Threading.Tasks;
using Xunit;

namespace Theseus.Core.Tests
{
    public class RequestDKGContractTests
    {


        [Fact]
        public async Task RequestDKG_4LoyalDKGNodes_RequestorReceives1DKGPub()
        {
            //Arrange
            var srwcService = CreateSrwc();
            var requestor = new Node(srwcService);
            var prover = new Node(srwcService);
            var dkgNode1 = new Node(srwcService);
            var dkgNode2 = new Node(srwcService);
            var dkgNode3 = new Node(srwcService);
            var dkgNode4 = new Node(srwcService);

            // srwcService.RegisterColleagues(new List)

            //Act
            await requestor.RequestDKG(prover.Id);

            //Assert
            var dkgPubs = requestor.GetDKGPubs();
            Assert.NotEmpty(dkgPubs);
        }

        private AdHocSrwcService CreateSrwc()
        {
            return new AdHocSrwcService();
        }
    }
}