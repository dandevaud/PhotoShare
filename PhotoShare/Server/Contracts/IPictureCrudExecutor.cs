using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;
using System.IO.Compression;

namespace PhotoShare.Server.Contracts
{
    public interface IPictureCrudExecutor
    {
        IReadOnlyCollection<PictureDto> GetGroupPictures(Guid groupId);
        T GetPicture<T>(Guid groupId, Guid pictureId) where T : PictureDto, new();
       IReadOnlyCollection<PictureDto> GetPictures(Guid groupId, List<Guid> pictureIds);
        Task UploadPicture(PictureUploadRequest request);

        Task<bool> DeletePicture(Guid groupId, Guid pictureId, Guid deletionKey);
        bool HasAdminAccess(Guid groupId, Guid pictureId, Guid deletionKey);
    }
}
