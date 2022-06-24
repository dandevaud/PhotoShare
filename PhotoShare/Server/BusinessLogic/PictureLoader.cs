using PhotoShare.Server.Contracts;
using PhotoShare.Shared;
using PhotoShare.Shared.Extension;
using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;
using System.IO.Compression;

namespace PhotoShare.Server.BusinessLogic
{
    public class PictureLoader : IPictureLoader
    {
        private readonly IPictureCrudExecutor _pictureCrudExecutor;
        private readonly IFileHandler _fileHandler;
        private readonly IEncryptionHandler _encryptionHandler;
        private readonly IGroupKeyRetriever _groupKeyRetriever;


        public PictureLoader(IPictureCrudExecutor pictureCrudExecutor, IFileHandler fileHandler, IEncryptionHandler encryptionHandler, IGroupKeyRetriever groupKeyRetriever)
        {
            this._pictureCrudExecutor = pictureCrudExecutor;
            _encryptionHandler = encryptionHandler;
            _fileHandler = fileHandler;
            _groupKeyRetriever = groupKeyRetriever;
        }
        public async Task<PictureLoadResponse> LoadPicture(Guid groupId, Guid pictureId)
        {
            var picture = _pictureCrudExecutor.GetPicture(groupId, pictureId);
            var key = _groupKeyRetriever.GetEncryptionKey(groupId);
            var stream = _encryptionHandler.DecryptStream(await _fileHandler.GetFromFile(picture.Path), key, picture.IV);
            return new PictureLoadResponse()
            {
                Stream = stream,
                ContentType = picture.ContentType,
                Name = picture.fileName
            };
            
        }



        public async Task<PictureLoadResponse> LoadPictures(Guid groupId, IReadOnlyCollection<Guid> pictures)
        {
            var ms = new MemoryStream();
            using var zippedStream = new ZipArchive(ms, ZipArchiveMode.Create, true);
            var nameCountDict = new Dictionary<string, int>();

                foreach (var picture in pictures)
                {
                    
                    var response = await LoadPicture(groupId, picture);
                    var fileName = nameCountDict.TryGetValue(response.Name, out var count) ? count+"_"+response.Name : nameCountDict.TryAdd(response.Name, 0) ? response.Name : response.Name;
                    nameCountDict[response.Name] = nameCountDict[response.Name] + 1;
                    var entry = zippedStream.CreateEntry(fileName);
                    using (var zipEntryStream = entry.Open())
                    {
                        await response.Stream.CopyToAsync(zipEntryStream);
                    }

                }
            zippedStream.Dispose();
            ms.Seek(0, SeekOrigin.Begin);
            return new PictureLoadResponse()
            {
                Stream = ms,
                ContentType = "application/zip"
            };

        }
    }
}
