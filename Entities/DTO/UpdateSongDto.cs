using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class UpdateSongDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public int TrackNumber { get; set; }
        public int AlbumId { get; set; }
    }
}
