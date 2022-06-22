using PhotoShare.Server.Contracts;
using System.Security.Cryptography;

namespace PhotoShare.Server.Files
{
    public class FileHandler : IFileHandler
    {
        public void DeleteFile(string filePath)
        {
           FileInfo fileInfo = new FileInfo(filePath);
            fileInfo.Delete();
        }

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
            var file = new FileInfo(filePath);
            if (!file?.Directory?.Exists ?? false) file.Directory.Create();
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                if (stream is CryptoStream)
                {
                    
                    var cs = ((CryptoStream)stream);
                    cs.CopyTo(fs);
                    if (!cs.HasFlushedFinalBlock)
                    {
                        await cs.FlushFinalBlockAsync();
                        stream.Close();
                    }
                }
                else
                {
                    await stream.CopyToAsync(fs);
                    await stream.FlushAsync();
                }
                await fs.FlushAsync();
                fs.Close();
            }


        }
    }
}
