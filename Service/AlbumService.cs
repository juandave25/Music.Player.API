using Infrastructure.Model;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;

        public AlbumService(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository;
        }

        public async Task<IEnumerable<Album>> GetAllAlbumsAsync()
        {
            return await _albumRepository.GetAlbumsWithDetailsAsync();
        }

        public async Task<Album> GetAlbumByIdAsync(int id)
        {
            return await _albumRepository.GetAlbumWithDetailsAsync(id);
        }

        public async Task<Album> CreateAlbumAsync(Album album)
        {
            await _albumRepository.AddAsync(album);
            return album;
        }

        public async Task UpdateAlbumAsync(Album album)
        {
            await _albumRepository.UpdateAsync(album);
        }

        public async Task DeleteAlbumAsync(int id)
        {
            var album = await _albumRepository.GetByIdAsync(id);
            if (album != null)
            {
                await _albumRepository.DeleteAsync(album);
            }
        }
    }
}
