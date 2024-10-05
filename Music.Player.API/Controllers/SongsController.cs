using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Music.Player.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongsController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            var songs = await _songService.GetAllSongsAsync();
            return Ok(songs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSong(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            return Ok(song);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Song>> CreateSong(Song song)
        {
            var createdSong = await _songService.CreateSongAsync(song);
            return CreatedAtAction(nameof(GetSong), new { id = createdSong.Id }, createdSong);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id, Song song)
        {
            if (id != song.Id)
            {
                return BadRequest();
            }

            await _songService.UpdateSongAsync(song);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            await _songService.DeleteSongAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/stream")]
        public async Task<ActionResult<string>> GetStreamingUrl(int id)
        {
            var url = await _songService.GetStreamingUrlAsync(id);
            if (string.IsNullOrEmpty(url))
            {
                return NotFound();
            }
            return Ok(url);
        }
    }
}
