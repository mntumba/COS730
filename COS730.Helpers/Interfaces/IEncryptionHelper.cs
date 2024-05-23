namespace COS730.Helpers.Interfaces
{
    public interface IEncryptionHelper
    {
        string GenerateOtp();
        (byte[] EncryptedMessage, byte[] EncryptedAesKey, byte[] IV) EncryptMessage(string message);
        string DecryptMessage(byte[] encryptedMessage, byte[] encryptedAesKey, byte[] iv);
        string EncryptCode(string code);
        bool VerifyCode(string code, string storedHashCode);
    }
}
