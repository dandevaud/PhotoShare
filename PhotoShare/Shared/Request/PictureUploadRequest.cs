using Microsoft.AspNetCore.Components.Forms;

namespace PhotoShare.Shared.Request
{
    public class PictureUploadRequest
    {
        public Guid GroupId { get; set; }
        public Guid UploaderKey { get; set; }
        public string? Uploader { get; set; }

        public byte[] Data { get; set; } = new byte[0];

        public string Name { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public long Size { get; set; }

        public string ContentType { get; set; }

        public PictureUploadRequest(Guid groupId, Guid uploaderKey, string? uploader, IBrowserFile file)
        {
            GroupId = groupId;
            UploaderKey = uploaderKey;
            Uploader = uploader;
            Name = file.Name;
            LastModified = file.LastModified;
            Size = file.Size;
            ContentType = file.ContentType;
        }
        public PictureUploadRequest() { }
    }
}
