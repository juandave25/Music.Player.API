using Infrastructure.Model;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistService(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<IEnumerable<Artist>> GetAllArtistsAsync()
        {
            return await _artistRepository.GetArtistsWithAlbumsAsync();
        }

        public async Task<Artist> GetArtistByIdAsync(int id)
        {
            return await _artistRepository.GetArtistWithAlbumsAsync(id);
        }

        public async Task<Artist> CreateArtistAsync(Artist artist)
        {
            await _artistRepository.AddAsync(artist);
            return artist;
        }

        public async Task UpdateArtistAsync(Artist artist)
        {
            await _artistRepository.UpdateAsync(artist);
        }

        public async Task DeleteArtistAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);
            if (artist != null)
            {
                await _artistRepository.DeleteAsync(artist);
            }
        }
    }
}
