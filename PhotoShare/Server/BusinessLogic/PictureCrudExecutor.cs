using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Configuration;
using PhotoShare.Server.Contracts;
using PhotoShare.Server.Database.Context;
using PhotoShare.Shared;
using PhotoShare.Shared.Extension;
using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;
using System.IO.Compression;
using System.Security.Cryptography;

namespace PhotoShare.Server.BusinessLogic
{
    public class PictureCrudExecutor : IPictureCrudExecutor
    {
        private readonly PhotoShareContext _context;
        private readonly IEncryptionHandler _encryptionHandler;
        private readonly IFileHandler _fileHandler;
        private readonly IConfiguration _configuration;

        public PictureCrudExecutor(PhotoShareContext context, IEncryptionHandler encryptionHandler, IFileHandler fileHandler, IConfiguration config)
        {
            _context = context;
            _encryptionHandler = encryptionHandler;
            _fileHandler = fileHandler;
            _configuration = config;

        }

        public async Task<bool> DeletePicture(Guid groupId, Guid pictureId, Guid deletionKey)
        {
            var picture = _context.Pictures.FirstOrDefault(p => p.Id == pictureId) ?? new Shared.Picture();
            if (picture.GroupId != groupId) return false;
            if (       HasAdminAccessToPicture(groupId, picture,  deletionKey))
            {
                _fileHandler.DeleteFile(picture.Path);
                _context.Pictures.Remove(picture);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public IReadOnlyCollection<PictureDto> GetGroupPictures(Guid groupId)
        {
            var pictures = _context.Pictures.Where(p => p.GroupId == groupId).Select(p => p.ToPictureDto()).ToList();
            return pictures;

        }

        public IReadOnlyCollection<PictureDto> GetPictures(Guid groupId, List<Guid> pictureIds)
        {
            var pictures = _context.Pictures.Where(p => p.GroupId == groupId && pictureIds.Contains(p.Id)).Select(p => p.ToPictureDto()).ToList();
            return pictures;

        }

        private async Task<PictureResponse> HandleMultiplePictures(Guid groupId, IQueryable<Shared.Picture> pictures)
        {
            var key = _context.GroupKeys.First(gk => gk.GroupId == groupId).EncryptionKey;
            var result = new PictureResponse();
            var resultBaseStream = new MemoryStream();
            var zipArchive = new ZipOutputStream(resultBaseStream) ;
            

                foreach (var picture in pictures)
            {
                var entry =  new ZipEntry(ZipEntry.CleanName(picture.fileName));
                zipArchive.PutNextEntry(entry);
                var buffer = new byte[4096];
                using var fileStream = await _fileHandler.GetFromFile(picture.Path);
                    using var stream = _encryptionHandler.DecryptStream(fileStream, key, picture.IV);
                StreamUtils.Copy(stream, zipArchive, buffer);
                zipArchive.CloseEntry();

                }
            
            await zipArchive.FlushAsync();
            result.Count = pictures.Count();
            resultBaseStream.Position = 0;
            result.Result = resultBaseStream;

            return result;
        }


      

        public PictureDto GetPictureDto(Guid groupId, Guid pictureId)
        {
            return GetPicture(groupId, pictureId).ToPictureDto();
        }

        public Picture GetPicture(Guid groupId, Guid pictureId) 
        {
            var picture = _context.Pictures.FirstOrDefault(p => p.Id == pictureId) ?? new Shared.Picture();
            if (picture.GroupId != groupId) return new Picture();

            return picture;
        }




        public async Task UploadPicture(PictureUploadRequest request)
        {
            var aes = Aes.Create();
            var key = _context.GroupKeys.First(gk => gk.GroupId == request.GroupId).EncryptionKey;
            if (key == null) throw new KeyNotFoundException($"No Key defined for {request.GroupId}");
            aes.Key = key;
            using var ms = new MemoryStream(request.Data);
            using var stream = _encryptionHandler.EncryptStream(ms,aes.Key,aes.IV);
            var path = $"{_configuration.GetValue<string>("FileSaveLocation")}/{request.GroupId}/{Guid.NewGuid()}";
            await _fileHandler.SaveToFile(path, stream);
            _context.Pictures.Add(new Shared.Picture()
            {
                Date = DateTime.UtcNow,
                IV = aes.IV,
                fileName = request.Name,
                Path = path,
                Uploader = request.Uploader,
                UploaderKey = request.UploaderKey,
                GroupId = request.GroupId,
                ContentType= request.ContentType
            });
            await _context.SaveChangesAsync();
        }

        private bool HasAdminAccessToPicture(Guid groupId, Picture picture, Guid deletionKey)
        {
            return _context.GroupKeys.FirstOrDefault(gk => gk.GroupId == groupId)?.AdminKey == deletionKey ||
                picture.UploaderKey == deletionKey;
        }

        public bool HasAdminAccess(Guid groupId, Guid pictureId, Guid deletionKey)
        {
            var picture = _context.Pictures.FirstOrDefault(p => p.Id == pictureId) ?? new Shared.Picture();
            if (picture.GroupId != groupId) return false;
            return HasAdminAccessToPicture(groupId, picture, deletionKey);
        }
    }
}
