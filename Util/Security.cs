using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Auctus.Util
{
    public static class Security
    {
        private static readonly int SALT_SIZE = 32;

        public static string Encrypt(string plainText, string cryptoPassword)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            
            using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(cryptoPassword, SALT_SIZE))
            {
                byte[] saltBytes = keyDerivationFunction.Salt;
                byte[] keyBytes = keyDerivationFunction.GetBytes(32);
                byte[] ivBytes = keyDerivationFunction.GetBytes(16);
                using (Aes aesAlg = Aes.Create())
                {
                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(keyBytes, ivBytes))
                    {
                        using (MemoryStream msEncrypt = new MemoryStream())
                        {
                            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                                {
                                    swEncrypt.Write(plainText);
                                }
                                byte[] cipherTextBytes = msEncrypt.ToArray();
                                Array.Resize(ref saltBytes, saltBytes.Length + cipherTextBytes.Length);
                                Array.Copy(cipherTextBytes, 0, saltBytes, SALT_SIZE, cipherTextBytes.Length);
                                return Convert.ToBase64String(saltBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string encryptedText, string cryptoPassword)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentNullException("encryptedText");
            
            byte[] bytesEncrypted = Convert.FromBase64String(encryptedText);
            byte[] saltBytes = bytesEncrypted.Take(SALT_SIZE).ToArray();
            byte[] encryptedTextBytes = bytesEncrypted.Skip(SALT_SIZE).Take(bytesEncrypted.Length - SALT_SIZE).ToArray();

            using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(cryptoPassword, saltBytes))
            {
                byte[] keyBytes = keyDerivationFunction.GetBytes(32);
                byte[] ivBytes = keyDerivationFunction.GetBytes(16);
                using (Aes aesAlg = Aes.Create())
                {
                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(keyBytes, ivBytes))
                    {
                        using (MemoryStream msDecrypt = new MemoryStream(encryptedTextBytes))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                                {
                                    return swDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static string Hash(string plainText, string saltText)
        {
            byte[] salt = Encoding.UTF8.GetBytes(saltText);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(plainText), salt, 7000, HashAlgorithmName.SHA512))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(salt.Length));
            }
        }
    }
}
