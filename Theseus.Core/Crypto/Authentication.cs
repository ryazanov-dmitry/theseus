using System.Security.Cryptography;
using Theseus.Core.Dto;
using Theseus.Core.Exceptions;

namespace Theseus.Core.Crypto
{

    public interface IAuthentication
    {
        void Verify(SignedObject beaconMessage);
    }

    public class Authentication : IAuthentication
    {
        private readonly IRsa rsa;

        public Authentication(IRsa rsa)
        {
            this.rsa = rsa;

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