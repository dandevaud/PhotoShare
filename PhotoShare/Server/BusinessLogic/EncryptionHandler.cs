using PhotoShare.Server.Contracts;
using System.Security.Cryptography;

namespace PhotoShare.Server.BusinessLogic
{
    public class EncryptionHandler : IEncryptionHandler
    {
        public Stream DecryptStream(Stream stream, Guid key)
        {
            using var aes = new AesManaged();
            aes.Key = key.ToByteArray();
            aes.IV = key.ToByteArray();
            return new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        }

        public Stream EncryptStream(Stream stream, Guid key)
        {
            using var aes = new AesManaged();
            aes.Key = key.ToByteArray();
            aes.IV = key.ToByteArray();
            return new CryptoStream(stream, aes.CreateEncryptor(),CryptoStreamMode.Read);

        }
    }
}
