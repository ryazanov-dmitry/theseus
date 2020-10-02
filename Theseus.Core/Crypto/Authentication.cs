using System.Security.Cryptography;
using Theseus.Core.Dto;
using Theseus.Core.Exceptions;

namespace Theseus.Core.Crypto
{

    public interface IAuthentication
    {
        void Verify(SignedObject beaconMessage);
        string Base64PublicKey();
        void Sign(SignedObject dkgRequest);
    }

    public class Authentication : IAuthentication
    {
        private readonly IRsa rsa;
        private RSAParameters rsaParams;

        public Authentication(IRsa rsa)
        {
            this.rsa = rsa;
            this.rsaParams = rsa.GenerateKeyPair();
        }

        public string Base64PublicKey()
        {
            return System.Convert.ToBase64String(this.rsaParams.Modulus);
        }

        public void Sign(SignedObject signedObject)
        {
            var signature = rsa.HashAndSign(signedObject.PlainData(), this.rsaParams);
            signedObject.Signature = signature;
            signedObject.Key = this.rsaParams.Modulus;
        }

        public void Verify(SignedObject signedObject)
        {
            if (!rsa.Verify(
                    signedObject.PlainData(),
                    signedObject.Signature,
                    new RSAParameters { Modulus = signedObject.Key, Exponent = rsa.Exponent }))
            {
                throw new AuthenticationException("Authentication of message failed.");
            }
        }
    }
}