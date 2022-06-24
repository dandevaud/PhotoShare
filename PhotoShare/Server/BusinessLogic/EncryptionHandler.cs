using PhotoShare.Server.Contracts;
using System.Security.Cryptography;

namespace PhotoShare.Server.BusinessLogic
{
    public class EncryptionHandler : IEncryptionHandler
    {
        public CryptoStream DecryptStream(Stream stream, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            return new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        }

        public CryptoStream EncryptStream(Stream stream, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            return new CryptoStream(stream, aes.CreateEncryptor(),CryptoStreamMode.Read);

        }
    }
}
