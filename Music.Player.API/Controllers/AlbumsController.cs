using Entities.DTO;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music.Player.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumService _albumService;

        public AlbumsController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbums()
        {
            var albums = await _albumService.GetAllAlbumsAsync();
            var albumDtos = albums.Select(a => new AlbumDto
            {
                Id = a.Id,
                ArtistId = a.ArtistId,
                ReleaseDate = a.ReleaseDate,
                ArtistName = a.Artist.Name,
                Title = a.Title

            });
            return Ok(albumDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlbumDto>> GetAlbum(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            var albumDto = new AlbumDto
            {
                Id = album.Id,
                ArtistId = album.ArtistId,
                ReleaseDate = album.ReleaseDate,
                ArtistName = album.Artist.Name,
                Title = album.Title

            };
            return Ok(albumDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<AlbumDto>> CreateAlbum(CreateAlbumDto albumDto)
        {
            var album = new Album
            {
                Title = albumDto.Title,
                ArtistId = albumDto.ArtistId,
                ReleaseDate = albumDto.ReleaseDate,
            };
            var createdAlbum = await _albumService.CreateAlbumAsync(album);
            return CreatedAtAction(nameof(GetAlbum), new { id = createdAlbum.Id }, createdAlbum);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(int id, UpdateAlbumDto albumDto)
        {
            if (id != albumDto.Id)
            {
                return BadRequest();
            }
            var album = new Album
            {
                Id  =  albumDto.Id,
                Title = albumDto.Title,
                ArtistId = albumDto.ArtistId,
                ReleaseDate = albumDto.ReleaseDate,
            };
            await _albumService.UpdateAlbumAsync(album);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            await _albumService.DeleteAlbumAsync(id);
            return NoContent();
        }
    }
}
