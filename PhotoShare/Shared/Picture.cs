using PhotoShare.Shared.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Shared
{
    public class Picture : PictureDto
    {
      
        public string Path { get; set; }
        public byte[] IV { get; set; }
        public Guid UploaderKey { get; set; }

        public PictureDto GetDto()
        {
            return (PictureDto) this;
        }
    }
}
