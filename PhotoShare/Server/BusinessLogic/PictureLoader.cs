using PhotoShare.Server.Contracts;
using PhotoShare.Shared;
using PhotoShare.Shared.Extension;
using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;

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
                ContentType = picture.ContentType
            };
            
        }
    }
}
