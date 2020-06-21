using System;
using System.Security.Cryptography;

namespace Theseus.Core.Crypto
{
    public class RSA
    {
        public RSAParams GenerateKeyPair()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters rsaKeyInfo = rsa.ExportParameters(false);
            return new RSAParams{
                
            };
        }
    }
}