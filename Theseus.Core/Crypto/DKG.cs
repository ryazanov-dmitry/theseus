using System;
using System.Linq;
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

        public const int primeConfidence = 10;
        private const int bits = 500;
        private const int g = 2;

        /// <summary>
        /// Pi chooses xi from Zq at random
        /// </summary>
        /// <returns></returns>
        public BigInteger GetElementFromGroup(BigInteger order)
        {
            var randomInt = RandomIntegerBelow(order);
            BigInteger gcd;
            do
            {
                randomInt++;
                gcd = randomInt.gcd(order);
            } while (gcd != 1);

            return randomInt;
        }

        public DKGParams1 DKGStep1()
        {
            var group_z = GenerateSubgroup();
            var x_i = GetElementFromGroup(group_z.q);
            var h_i = new BigInteger(g).modPow(x_i, group_z.q);
            var r_i = "random string"; //TODO: change.
            var c_i = AES.EncryptAes(r_i, h_i.getBytes().Take(16).ToArray());

            return new DKGParams1
            {
                q = group_z.q,
                p = group_z.p,
                x_i = x_i,
                h_i = h_i,
                r_i = r_i,
                c_i = c_i
            };
        }

        public (BigInteger p, BigInteger q) GenerateSubgroup()
        {
            var q = GenerateLargePrime();
            BigInteger p;
            BigInteger r = 2;
            p = q * r + 1;

            while (!p.isProbablePrime(primeConfidence))
            {
                p = (q * (++r)) + 1;
            }

            return (p, q);
        }


        public BigInteger GenerateLargePrime()
        {
            // var keys = new RSA().GenerateKeyPair();
            // ForceSignBitToPositive(keys.P);
            // return new BigInteger(keys.P);
            return BigInteger.genPseudoPrime(bits, primeConfidence, new Random());
        }


        public static BigInteger RandomIntegerBelow(BigInteger N)
        {
            byte[] bytes = N.getBytes();
            BigInteger R;
            var random = new Random();
            do
            {
                random.NextBytes(bytes);
                ForceSignBitToPositive(bytes);
                R = new BigInteger(bytes);
            } while (R >= N);

            return R;
        }

        private static void ForceSignBitToPositive(byte[] bytes)
        {
            bytes[bytes.Length - 1] &= (byte)0x7F;
        }
    }

    public class DKGParams1
    {
        public BigInteger q { get; internal set; }
        public BigInteger p { get; internal set; }
        public byte[] c_i { get; internal set; }
        public string r_i { get; internal set; }
        public BigInteger h_i { get; internal set; }
        public BigInteger x_i { get; internal set; }
    }
}