using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace COS730.Helpers
{
    public class EncryptionHelper
    {
        private static string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);

            return Base32Encoding.ToString(key);
        }

        public static string GenerateOtp()
        {
            var secretKey = GenerateSecretKey();

            var secretKeyBytes = Base32Encoding.ToBytes(secretKey);

            var totp = new Totp(secretKeyBytes);

            return totp.ComputeTotp();
        }

        public static string EncryptCode(string encryptString)
        {
            string EncryptionKey = "ZWDJOVMPQ4RGPG7E5P7VUQP4JNMEPDM7BMHPTZOXD6SGDAJVAKRTGZD2YIPGLAVE";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new(EncryptionKey, Base32Encoding.ToBytes(EncryptionKey));
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                encryptString = Convert.ToBase64String(ms.ToArray());
            }

            return encryptString;
        }

        public static bool IsValid(string password, string hashPassword)
        {
            var inputPassword = EncryptCode(password);

            return string.Equals(hashPassword, inputPassword);
        }
    }
}
