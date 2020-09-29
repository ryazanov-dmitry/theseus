using System.Numerics;
using Theseus.Core.Crypto;
using Xunit;
using Xunit.Abstractions;

namespace Theseus.Core.Tests
{
    public class DKGTests
    {
        private readonly ITestOutputHelper output;

        public DKGTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CreateRandomX_HappyPath()
        {
            //Arrange
            var dkg = new DKG();
            var q = dkg.GenerateLargePrime();


            //Act
            for (int i = 0; i < 100; i++)
            {
                var x = dkg.CreateRandomX(q);
                output.WriteLine(x.ToString());
                //Assert
                Assert.True(q > x);
                Assert.True(BigInteger.GreatestCommonDivisor(x, q) == 1);
            }
        }
    }
}