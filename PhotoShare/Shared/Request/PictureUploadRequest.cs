using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Shared.Request
{
    public class PictureUploadRequest
    {
        public Guid GroupId { get; set; }
        public string fileName { get; set; }
        public Guid UploaderKey { get; set; }
        public string? Uploader { get; set; }
        public Stream pictureStream { get; set; }
    }
}
