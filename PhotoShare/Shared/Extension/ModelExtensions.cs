using PhotoShare.Shared.Request;
using PhotoShare.Shared.Response;

namespace PhotoShare.Shared.Extension
{
    public static class ModelExtensions
    {
        public static PictureDto ToPictureDto(this Picture pic)
        {
            return new PictureDto(pic);
        }

    }
}
