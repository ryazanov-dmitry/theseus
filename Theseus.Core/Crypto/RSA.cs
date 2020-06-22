using System;
using System.Security.Cryptography;

namespace Theseus.Core.Crypto
{
    public interface IRsa
    {
        RSAParameters GenerateKeyPair();
        byte[] HashAndSign(byte[] dataToSign, RSAParameters key);
        bool Verify(byte[] originalData, byte[] signature, RSAParameters key);
    }

    public class RSA : IRsa
    {
        public RSAParameters GenerateKeyPair()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters rsaKeyInfo = rsa.ExportParameters(true);
            return rsaKeyInfo;
        }

        public byte[] HashAndSign(byte[] dataToSign, RSAParameters key)
        {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
            RSAalg.ImportParameters(key);
            return RSAalg.SignData(dataToSign, new SHA1CryptoServiceProvider());
        }

        public bool Verify(byte[] originalData, byte[] signature, RSAParameters key)
        {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
            RSAalg.ImportParameters(key);
            return RSAalg.VerifyData(originalData, new SHA1CryptoServiceProvider(), signature);
        }
    }
}