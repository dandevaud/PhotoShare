using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;
using System.IO.Compression;

namespace PhotoShare.Server.Contracts
{
    public interface IPictureCrudExecutor
    {
        Task<PictureResponse> GetGroupPictures(Guid groupId);
        Task<PictureResponse> GetPicture(Guid groupId, Guid pictureId);
        Task<PictureResponse> GetPictures(Guid groupId, List<Guid> pictureIds);
        Task UploadPicture(PictureUploadRequest request);

        Task<bool> DeletePicture(Guid groupId, Guid pictureId, Guid deletionKey);
    }
}
