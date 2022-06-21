namespace PhotoShare.Server.Contracts
{
    public interface IGroupKeyRetriever
    {
        public Guid GetAdminKey(Guid groupId);
        public byte[] GetEncryptionKey(Guid groupId);
    }
}
