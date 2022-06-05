using PhotoShare.Server.Contracts;

namespace PhotoShare.Server.Files
{
    public class FileHandler : IFileHandler
    {
        public async Task<Stream> GetFromFile(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var memoryStream = new MemoryStream();
            await fs.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task SaveToFile(string filePath, Stream stream)
        {
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            await stream.CopyToAsync(fs);
            fs.Flush();
            fs.Close();

        }
    }
}
