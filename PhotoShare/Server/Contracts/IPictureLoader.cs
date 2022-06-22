using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;

namespace PhotoShare.Server.Contracts
{
    public interface IPictureLoader
    {
        Task<PictureLoadResponse> LoadPicture(Guid groupId, Guid pictureId);
    }
}
