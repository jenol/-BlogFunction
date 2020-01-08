using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Service.DomainObjects
{
    internal class Email
    {
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("Jeno Laszlo");

        private string _emailText;
        //private const string EncryptionKey = "abc123";

        private Email(string emailText)
        {
            _emailText = emailText;
        }

        public static bool TryParse(string emailText, out Email email)
        {
            if (RegexUtilities.IsValidEmail(emailText))
            {
                email = new Email(emailText);
                return true;
            }

            email = null;
            return false;
        }

        public string EncryptedEmail(string encryptionKey) => EncryptEmail(_emailText, encryptionKey);

        public string DecryptedEmail(string encryptionKey) => DecryptEmail(_emailText, encryptionKey);

        private static string EncryptEmail(string email, string encryptionKey)
        {
            var clearBytes = Encoding.Unicode.GetBytes(email);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, Salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    email = Convert.ToBase64String(ms.ToArray());
                }
            }

            return email;
        }

        public static string DecryptEmail(string email, string encryptionKey)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            email = email.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(email);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, Salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    email = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return email;
        }

        public override string ToString() => _emailText;
    }
}
