using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Shared.Response
{
    public class PictureLoadResponse
    {
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
        public string? Name { get; set; }
    }
}
