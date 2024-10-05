using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class SongRepository : Repository<Song>, ISongRepository
    {
        public SongRepository(Context context) : base(context) { }

        public async Task<IEnumerable<Song>> GetSongsWithDetailsAsync()
        {
            return await _context.Songs
                .Include(s => s.Album)
                    .ThenInclude(a => a.Artist)
                .Include(s => s.Genres)
                .Include(s => s.MediaFile)
                .ToListAsync();
        }

        public async Task<Song> GetSongWithDetailsAsync(int id)
        {
            return await _context.Songs
                .Include(s => s.Album)
                    .ThenInclude(a => a.Artist)
                .Include(s => s.Genres)
                .Include(s => s.MediaFile)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
