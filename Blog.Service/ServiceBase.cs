using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Service
{
    public abstract class ServiceBase
    {
        protected ServiceBase(string encryptionKey, string salt)
        {
            EncryptionKey = encryptionKey;
            Salt = Encoding.UTF8.GetBytes(salt);
        }

        protected string EncryptionKey { get; }
        protected byte[] Salt { get; }

        private Aes Create(string encryptionKey)
        {
            var encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(encryptionKey, Salt);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            return encryptor;
        }

        protected byte[] GetEncryptedBytes(string value)
        {
            var clearBytes = Encoding.UTF8.GetBytes(value);
            using (var encryptor = Create(EncryptionKey))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                    }

                    return ms.ToArray();
                }
            }
        }

        protected string GetEncryptedText(string value) => Convert.ToBase64String(GetEncryptedBytes(value)).Replace("/", "*");
        
        protected string GetDecryptedText(byte[] cipherBytes)
        {
            if (cipherBytes == null || !cipherBytes.Any())
            {
                return null;
            }

            using (var encryptor = Create(EncryptionKey))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        protected string GetDecryptedText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            
            return GetDecryptedText(Convert.FromBase64String(value.Replace("*", "/").Replace(" ", "+")));
        }
    }
}
