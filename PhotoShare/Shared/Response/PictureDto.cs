using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Shared.Response
{
    public class PictureDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string fileName { get; set; }
        public DateTime Date { get; set; }
        public string? Uploader { get; set; }

        public PictureDto() { }
        public PictureDto (Picture pic)
        {
            Id = pic.Id;
            Uploader = pic.Uploader;
            GroupId = pic.GroupId;
            fileName = pic.fileName;
            Date = pic.Date;

        }

       
    }
}
