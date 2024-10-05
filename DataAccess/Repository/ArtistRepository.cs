using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ArtistRepository : Repository<Artist>, IArtistRepository
    {
        public ArtistRepository(Context context) : base(context) { }

        public async Task<IEnumerable<Artist>> GetArtistsWithAlbumsAsync()
        {
            return await _context.Artists
                .Include(a => a.Albums)
                .ToListAsync();
        }

        public async Task<Artist> GetArtistWithAlbumsAsync(int id)
        {
            return await _context.Artists
                .Include(a => a.Albums)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
