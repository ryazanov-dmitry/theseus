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
        public void GetElementFromGroup_HappyPath()
        {
            //Arrange
            var dkg = new DKG();
            var q = dkg.GenerateLargePrime();


            //Act
            for (int i = 0; i < 100; i++)
            {
                var x = dkg.GetElementFromGroup(q);
                output.WriteLine(x.ToString());
                //Assert
                Assert.True(q > x);
                Assert.True(x.gcd(q) == 1);
            }
        }

        [Fact]
        public void GenerateSubgroup_HappyPath()
        {
            //Arrange
            var dkg = new DKG();
            for (int i = 0; i < 10; i++)
            {
                //Act
                var group = dkg.GenerateSubgroup();

                //Assert
                Assert.True(group.q.isProbablePrime(DKG.primeConfidence));
                Assert.True(group.p.isProbablePrime(DKG.primeConfidence));

                Assert.True((group.p - 1) % group.q == 0);
            }
        }

        [Fact]
        public void DKGStep1()
        {
            var dkg = new DKG();

            dkg.DKGStep1();
        }
    }
}