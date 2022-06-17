using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Configuration;
using PhotoShare.Server.Contracts;
using PhotoShare.Server.Database.Context;
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
        private readonly string _directory = Environment.CurrentDirectory;

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
            if(_context.GroupKeys.FirstOrDefault(gk => gk.GroupId == groupId)?.AdminKey == deletionKey ||
                picture.UploaderKey == deletionKey)
            {
                _fileHandler.DeleteFile(picture.Path);
                _context.Pictures.Remove(picture);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<PictureResponse> GetGroupPictures(Guid groupId)
        {
            var pictures = _context.Pictures.Where(p => p.GroupId == groupId);
            return await HandleMultiplePictures(groupId, pictures);

        }

        public async Task<PictureResponse> GetPictures(Guid groupId, List<Guid> pictureIds)
        {
            var pictures = _context.Pictures.Where(p => p.GroupId == groupId && pictureIds.Contains(p.Id));
            return await HandleMultiplePictures(groupId, pictures);

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

        public async Task<PictureResponse> GetPicture(Guid groupId, Guid pictureId)
        {
            var picture = _context.Pictures.First(p => p.Id == pictureId);
            if (picture.GroupId != groupId) return new PictureResponse() { Count = 0 };
            var key = _context.GroupKeys.First(gk => gk.GroupId == groupId).EncryptionKey;
            var stream = _encryptionHandler.DecryptStream(await _fileHandler.GetFromFile(picture.Path), key, picture.IV);
            return new PictureResponse()
            {
                Count = 1,
                Result = stream
            };
        }

        public async Task UploadPicture(PictureUploadRequest request)
        {
            var aes = Aes.Create();
            var key = _context.GroupKeys.First(gk => gk.GroupId == request.GroupId).EncryptionKey;
            if (key == null) throw new KeyNotFoundException($"No Key defined for {request.GroupId}");
            aes.Key = key;
            using var stream = _encryptionHandler.EncryptStream(request.pictureStream,aes.Key,aes.IV);
            await stream.FlushFinalBlockAsync();
            var path = $"{(String.IsNullOrEmpty(_configuration.GetValue<string>("FileSaveLocation"))?_directory :_configuration.GetValue<string>("FileSaveLocation"))}/{request.GroupId}/{request.pictureStream}";
            using var fs = _fileHandler.SaveToFile(path, stream);
            _context.Pictures.Add(new Shared.Picture()
            {
                Date = DateTime.UtcNow,
                IV = aes.IV,
                fileName = request.fileName,
                Path = path,
                Uploader = request.Uploader,
                UploaderKey = request.UploaderKey,
                GroupId = request.GroupId
            });
            await _context.SaveChangesAsync();
        }
    }
}
