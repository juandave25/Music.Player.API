using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class MediaFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string S3BucketName { get; set; }

        [Required]
        public string S3ObjectKey { get; set; }

        public long FileSizeBytes { get; set; }

        [MaxLength(50)]
        public string ContentType { get; set; }

        public DateTime UploadDate { get; set; }

        public int SongId { get; set; }
        public virtual Song Song { get; set; }
    }
}
