using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Universal
{
    internal sealed class Cryptography
    {
        private static readonly byte[] iv = new byte[16] { 0xAA, 0xF2, 0x5A, 0x21, 0xC6, 0xE1, 0xFF, 0x23, 0x12, 0x10, 0x67, 0x29, 0x4B, 0xCD, 0x1E, 0x8A };
        private static readonly string password = "8214K(%!DG1SK:@Le9twIW5EOnz456X(9#^6%*)";
        public static string Encrypt(string message)
        {
            System.Security.Cryptography.SHA256 mySHA256 = System.Security.Cryptography.SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));
            return EncryptString(message, key, iv);
        }
        public static string Decrypt(string encrypted)
        {
            System.Security.Cryptography.SHA256 mySHA256 = System.Security.Cryptography.SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));
            return DecryptString(encrypted, key, iv);
        }
        private static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            return cipherText;
        }
        private static string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string plainText = String.Empty;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }
            return plainText;
        }
    }
}