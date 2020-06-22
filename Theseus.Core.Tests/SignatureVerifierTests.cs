using System;
using System.Text;
using Theseus.Core.Crypto;
using Xunit;

namespace Theseus.Core.Tests
{
    public class SignatureVerifierTests
    {


        [Fact]
        public void Verify_HappyPath()
        {
            //Arrange
            var rsa = new RSA();
            var key = rsa.GenerateKeyPair();
            byte[] signature = rsa.HashAndSign(key.Modulus, key);

            //Act
            var isValidSignature = rsa.Verify(key.Modulus, signature, key);

            //Assert
            Assert.True(isValidSignature);
        }
    }
}