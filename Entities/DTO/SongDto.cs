using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public int TrackNumber { get; set; }
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public string ArtistName { get; set; }
    }
}
