using PhotoShare.Shared.Response;

namespace PhotoShare.Client.Shared.Models
{
    public class PictureUIDto
    {
       public PictureDto picture { get; set; }

       public bool isSelected { get; set; }
    }
}
