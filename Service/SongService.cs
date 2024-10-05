using Amazon.S3;
using Amazon.S3.Model;
using Infrastructure.Model;
using Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public SongService(ISongRepository songRepository, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _songRepository = songRepository;
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"];
        }

        public async Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            return await _songRepository.GetSongsWithDetailsAsync();
        }

        public async Task<Song> GetSongByIdAsync(int id)
        {
            return await _songRepository.GetSongWithDetailsAsync(id);
        }

        public async Task<Song> CreateSongAsync(Song song)
        {
            await _songRepository.AddAsync(song);
            return song;
        }

        public async Task UpdateSongAsync(Song song)
        {
            await _songRepository.UpdateAsync(song);
        }

        public async Task DeleteSongAsync(int id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            if (song != null)
            {
                await _songRepository.DeleteAsync(song);
            }
        }

        public async Task<string> GetStreamingUrlAsync(int songId)
        {
            var song = await _songRepository.GetSongWithDetailsAsync(songId);
            if (song?.MediaFile == null)
            {
                return null;
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = song.MediaFile.S3ObjectKey,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}
