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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Key { get; set; }
    }
}
