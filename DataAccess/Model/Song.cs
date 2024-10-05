using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.Model
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public int TrackNumber { get; set; }

        [MaxLength(10)]
        public string Duration { get; set; }

        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }

        public virtual MediaFile MediaFile { get; set; }
    }
}
