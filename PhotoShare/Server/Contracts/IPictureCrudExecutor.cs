using PhotoShare.Shared;
using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;
using System.IO.Compression;

namespace PhotoShare.Server.Contracts
{
    public interface IPictureCrudExecutor
    {
        IReadOnlyCollection<PictureDto> GetGroupPictures(Guid groupId);
        Picture GetPicture(Guid groupId, Guid pictureId);
        PictureDto GetPictureDto(Guid groupId, Guid pictureId);
       IReadOnlyCollection<PictureDto> GetPictures(Guid groupId, List<Guid> pictureIds);
        Task UploadPicture(PictureUploadRequest request);

        Task<bool> DeletePicture(Guid groupId, Guid pictureId, Guid deletionKey);
        bool HasAdminAccess(Guid groupId, Guid pictureId, Guid deletionKey);
    }
}
