using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Shared.Response
{
    public class PictureResponse
    {
        public int Count { get; set; }
        public Stream Result { get; set; }
    }
}
