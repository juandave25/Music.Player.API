using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AlbumRepository : Repository<Album>, IAlbumRepository
    {
        public AlbumRepository(Context context) : base(context) { }

        public async Task<IEnumerable<Album>> GetAlbumsWithDetailsAsync()
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Songs)
                .ToListAsync();
        }

        public async Task<Album> GetAlbumWithDetailsAsync(int id)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
