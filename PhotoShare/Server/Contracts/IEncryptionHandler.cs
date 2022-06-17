using System.Security.Cryptography;

namespace PhotoShare.Server.Contracts
{
    public interface IEncryptionHandler
    {

        public CryptoStream EncryptStream(Stream stream, byte[] key, byte[] iv);
        public CryptoStream DecryptStream(Stream stream, byte[] key, byte[] iv);
    }
}
