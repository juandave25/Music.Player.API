using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Entities.DTO;

namespace Music.Player.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistsController : Controller
    {
        private readonly IArtistService _artistService;

        public ArtistsController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetArtists()
        {
            var artists = await _artistService.GetAllArtistsAsync();
            return Ok(artists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArtistDto>> GetArtist(int id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return Ok(artist);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Artist>> CreateArtist(CreateArtistDto artistDto)
        {
            var artist = new Artist
            {
                 Name  = artistDto.Name,
            };
            var createdArtist = await _artistService.CreateArtistAsync(artist);
            return CreatedAtAction(nameof(GetArtist), new { id = createdArtist.Id }, createdArtist);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArtist(int id, UpdateArtistDto artistDto)
        {
            if (id != artistDto.Id)
            {
                return BadRequest();
            }
            var artist = new Artist { 
                Id  = id,
                Name = artistDto.Name, 
            };
            await _artistService.UpdateArtistAsync(artist);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            await _artistService.DeleteArtistAsync(id);
            return NoContent();
        }
    }
}
