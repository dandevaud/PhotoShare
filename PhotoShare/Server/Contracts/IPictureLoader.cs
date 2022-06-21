using PhotoShare.Shared.Response;

namespace PhotoShare.Server.Contracts
{
    public interface IPictureLoader
    {
        Task<Stream> LoadPicture(Guid groupId, Guid pictureId);
    }
}
