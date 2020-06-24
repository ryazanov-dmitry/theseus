using System.Text;
using Newtonsoft.Json;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Xunit;
using Xunit.Abstractions;

namespace Theseus.Core.Tests.Dto
{
    public class DtoTests
    {
        private readonly ITestOutputHelper o;
        public DtoTests(ITestOutputHelper testOutputHelper)
        {
            this.o = testOutputHelper;

        }

        [Fact]
        public void SignedObject()
        {

            //Arrange
            var rsa = new RSA();
            var key = rsa.GenerateKeyPair();

            var signedBeacon = new Beacon
            {
                Id = System.Convert.ToBase64String(key.Modulus)
            };
            var originalPlainData = signedBeacon.PlainData();
            signedBeacon.Signature = rsa.HashAndSign(originalPlainData, key);
            signedBeacon.Key = key.Modulus;


            //Act
            var json = JsonConvert.SerializeObject(signedBeacon);
            var beacon = JsonConvert.DeserializeObject<Beacon>(json);
            var isCorrectSignature = rsa.Verify(
                    beacon.PlainData(), 
                    beacon.Signature, 
                    new System.Security.Cryptography.RSAParameters { Modulus = beacon.Key, Exponent = key.Exponent });

            //Assert
            Assert.Equal(originalPlainData, beacon.PlainData());
            Assert.True(isCorrectSignature, "Incorrect signature.");
        }
    }
}