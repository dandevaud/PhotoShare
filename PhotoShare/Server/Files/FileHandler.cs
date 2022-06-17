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
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            await stream.CopyToAsync(fs);


            if (stream is CryptoStream)
            {
                if (((CryptoStream)stream).HasFlushedFinalBlock) return;
                await ((CryptoStream)stream).FlushFinalBlockAsync();
                stream.Close();
            }
            else
            {

                fs.Flush();
                fs.Close();
            }
            
            
            

        }
    }
}
