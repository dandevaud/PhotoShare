using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Shared.Response
{
    public class GroupCreationResponse
    {
        public Group Group { get; set; }
        public Guid AdministrationKey { get; set; }
    }
}
