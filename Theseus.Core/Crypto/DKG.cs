using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Theseus.Core.Crypto
{
    /// <summary>
    /// p,q large primes; q divides p-1
    /// Gq - unique subgroup of Zp* of order q
    /// g - generator of Gq
    /// for all a and b != 1 in Gq discrete log of a 
    ///     with respect to b is defined log_b(a)
    /// easy to test if a from Zp* is in Gq since
    ///     a^q = 1
    /// </summary>
    public class DKG
    {
        /// <summary>
        /// Pi chooses xi from Zq at random
        /// </summary>
        /// <returns></returns>
        public BigInteger CreateRandomX(BigInteger q)
        {
            var randomInt = RandomIntegerBelow(q);
            BigInteger gcd;
            do
            {
                randomInt++;
                gcd = BigInteger.GreatestCommonDivisor(randomInt, q);
            } while (gcd != 1);

            return randomInt;
        }

        public BigInteger GenerateLargePrime()
        {
            var keys = new RSA().GenerateKeyPair();
            return new BigInteger(keys.P);
        }

        public static BigInteger RandomIntegerBelow(BigInteger N)
        {
            byte[] bytes = N.ToByteArray();
            BigInteger R;
            var random = new Random();
            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; //force sign bit to positive
                R = new BigInteger(bytes);
            } while (R >= N);

            return R;
        }


    }
}