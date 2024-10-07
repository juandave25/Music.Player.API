using Amazon.S3.Model;
using Amazon.S3;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.DTO;
using System.Linq;

namespace Music.Player.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly IAmazonS3 _s3Client;
        private readonly IAwsConfigurationService _awsConfigService;

        public SongsController(ISongService songService, IAmazonS3 s3Client, IAwsConfigurationService awsConfigService)
        {
            _songService = songService;
            _s3Client = s3Client;
            _awsConfigService = awsConfigService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetSongs()
        {
            var songs = await _songService.GetAllSongsAsync();
            var songDtos = songs.Select(s => new SongDto
            {
                Id = s.Id,
                Title = s.Title,
                Duration = s.Duration,
                TrackNumber = s.TrackNumber,
                AlbumId = s.AlbumId,
                AlbumTitle = s.Album?.Title,
                ArtistName = s.Album?.Artist?.Name
            });
            return Ok(songDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SongDto>> GetSong(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            var songDto = new SongDto
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                TrackNumber = song.TrackNumber,
                AlbumId = song.AlbumId,
                AlbumTitle = song.Album?.Title,
                ArtistName = song.Album?.Artist?.Name
            };
            return Ok(songDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<SongDto>> CreateSong(CreateSongDto songDto)
        {
            var song = new Song
            {
                Title = songDto.Title,
                Duration = songDto.Duration,
                TrackNumber = songDto.TrackNumber,
                AlbumId = songDto.AlbumId
            };
            var createdSong = await _songService.CreateSongAsync(song);
            return CreatedAtAction(nameof(GetSong), new { id = createdSong.Id }, createdSong);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id, UpdateSongDto songDto)
        {
            if (id != songDto.Id)
            {
                return BadRequest();
            }

            var song = new Song
            {
                Id = id,
                Title = songDto.Title,
                Duration = songDto.Duration,
                TrackNumber = songDto.TrackNumber,
                AlbumId = songDto.AlbumId
            };

            await _songService.UpdateSongAsync(song);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            if (song.MediaFile != null)
            {
                await DeleteSongFileFromS3(song.MediaFile);
            }

            await _songService.DeleteSongAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadSong(IFormFile file, [FromForm] UploadSongDto songDto)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var bucketName = await _awsConfigService.GetBucketNameAsync();
            var fileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";

            using (var stream = file.OpenReadStream())
            {
                var uploadRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = file.ContentType
                };

                await _s3Client.PutObjectAsync(uploadRequest);
            }

            var mediaFile = new MediaFile
            {
                S3BucketName = bucketName,
                S3ObjectKey = fileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                UploadDate = DateTime.UtcNow
            };

            var song = new Song
            {
                Title = songDto.Title,
                Duration = songDto.Duration,
                TrackNumber = songDto.TrackNumber,
                AlbumId = songDto.AlbumId
            };

            var createdSong = await _songService.CreateSongAsync(song);
            createdSong = await _songService.AddMediaFileToSongAsync(createdSong.Id, mediaFile);

            return CreatedAtAction(nameof(GetSong), new { id = createdSong.Id }, createdSong);
        }

        [HttpGet("{id}/stream")]
        public async Task<IActionResult> StreamSong(int id)
        {
            var Url = await _songService.GetStreamingUrlAsync(id);

            return Ok(new { StreamUrl = Url });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/file")]
        public async Task<IActionResult> DeleteSongFile(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song?.MediaFile == null)
                return NotFound();

            await DeleteSongFileFromS3(song.MediaFile);
            await _songService.RemoveMediaFileFromSongAsync(id);

            return NoContent();
        }

        private async Task DeleteSongFileFromS3(MediaFile mediaFile)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = mediaFile.S3BucketName,
                Key = mediaFile.S3ObjectKey
            };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }
    }
}
