namespace PhotoShare.Server.Contracts
{
    public interface IFileHandler
    {

        public Task<Stream> GetFromFile(string filePath);
        public Task SaveToFile(string filePath, Stream stream);

        public void DeleteFile(string filePath);
    }
}
