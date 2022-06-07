using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Shared
{
    public class GroupKey
    {
        public Guid GroupId { get; set; }
       
        public KeyType KeyType { get; set; }

        public Guid Key { get; set; }
    }

    public enum KeyType
    {
        Encryption,
        Administration
    }
}
