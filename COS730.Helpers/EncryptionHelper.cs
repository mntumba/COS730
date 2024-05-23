using COS730.Helpers.Interfaces;
using COS730.Models.Settings;
using Microsoft.Extensions.Options;
using OtpNet;
using System.Security.Cryptography;
using System.Text;

namespace COS730.Helpers
{
    public class EncryptionHelper : IEncryptionHelper
    {
        private readonly EncryptionSettings _encryptionSettings;

        public EncryptionHelper(IOptions<EncryptionSettings> encryptionSettings)
        {
            _encryptionSettings = encryptionSettings.Value;
        }

        private (byte[] iv, byte[] encryptedAesKey) EncryptAesKey(Aes aes)
        {
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using var rsa = new RSACryptoServiceProvider();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(_encryptionSettings.PublicKey!), out _);

            byte[] encryptedAesKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);

            return (iv, encryptedAesKey);
        }

        public string GenerateOtp()
        {
            byte[] encryptedAesKey;

            using var aes = Aes.Create();
            (_, encryptedAesKey) = EncryptAesKey(aes);

            var secretKey = Base32Encoding.ToString(encryptedAesKey);

            var secretKeyBytes = Base32Encoding.ToBytes(secretKey);

            var totp = new Totp(secretKeyBytes);

            return totp.ComputeTotp();
        }

        public (byte[] EncryptedMessage, byte[] EncryptedAesKey, byte[] IV) EncryptMessage(string message)
        {
            byte[] encryptedData;
            byte[] encryptedAesKey;
            byte[] iv;

            using (var aes = Aes.Create())
            {
                (iv, encryptedAesKey) = EncryptAesKey(aes);

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(message);
                }

                encryptedData = ms.ToArray();
            }

            return (encryptedData, encryptedAesKey, iv);
        }

        public string DecryptMessage(byte[] encryptedMessage, byte[] encryptedAesKey, byte[] iv)
        {
            string decryptedData;

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(_encryptionSettings.PrivateKey!), out _);
                var aesKey = rsa.Decrypt(encryptedAesKey, RSAEncryptionPadding.Pkcs1);

                using var aes = Aes.Create();
                aes.Key = aesKey;
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(encryptedMessage);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs);
                decryptedData = reader.ReadToEnd();
            }

            return decryptedData;
        }

        public string EncryptCode(string code)
        {
            byte[] saltBytes = Encoding.Unicode.GetBytes(_encryptionSettings.EncryptionKey!);

            using var deriveBytes = new Rfc2898DeriveBytes(code, saltBytes, 10000);
            byte[] hashBytes = deriveBytes.GetBytes(20);
            string hashCode = Convert.ToBase64String(hashBytes);

            return hashCode;
        }

        public bool VerifyCode(string code, string storedHashCode)
        {
            return EncryptCode(code) == storedHashCode;
        }
    }
}
