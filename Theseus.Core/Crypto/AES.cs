using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Theseus.Core.Crypto
{
    public class AES
    {
        public static byte[] EncryptAes(string raw, byte[] key)
        {
            using var aes = new AesManaged();
            return Encrypt(raw, key, aes.IV);
        }

        public static string DecryptAes(string m, byte[] key)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(m);
            var vector = bytes.TakeLast(16);
            return Decrypt(bytes.Take(bytes.Length - 16).ToArray(), key, vector.ToArray());
        }

        static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using var aes = new AesManaged();
            var encryptor = aes.CreateEncryptor(Key, IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(plainText);
            encrypted = ms.ToArray();
            return encrypted.Concat(IV).ToArray();
        }
        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            using var aes = new AesManaged();
            var decryptor = aes.CreateDecryptor(Key, IV);  
            using var ms = new MemoryStream(cipherText);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);   
            using var reader = new StreamReader(cs);
            plaintext = reader.ReadToEnd();
            return plaintext;
        }
    }
}