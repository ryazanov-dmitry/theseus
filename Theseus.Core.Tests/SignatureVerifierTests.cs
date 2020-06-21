using Theseus.Core.Crypto;
using Xunit;

namespace Theseus.Core.Tests
{
    public class SignatureVerifierTests
    {


        [Fact]
        public void Verify()
        {
            //Arrange
            var rsa = new RSA();
            var key = rsa.GenerateKeyPair();
            var publicKeyToSign = key.Exponent 
            //Act

            rsa.Verify(string dataToVerify, string signature, )

            //Assert
        }
    }
}