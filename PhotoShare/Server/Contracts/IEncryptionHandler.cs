namespace PhotoShare.Server.Contracts
{
    public interface IEncryptionHandler
    {

        public Stream EncryptStream(Stream stream, Guid key);
        public Stream DecryptStream(Stream stream, Guid key);
    }
}
