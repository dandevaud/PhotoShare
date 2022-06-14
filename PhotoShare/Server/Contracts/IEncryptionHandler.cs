namespace PhotoShare.Server.Contracts
{
    public interface IEncryptionHandler
    {

        public Stream EncryptStream(Stream stream, byte[] key, byte[] iv);
        public Stream DecryptStream(Stream stream, byte[] key, byte[] iv);
    }
}
