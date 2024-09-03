// Services/FileService.cs
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StorageMicroservice.Models;
using StorageMicroservice.Data;

namespace StorageMicroservice.Services
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _storageRoot;

        public FileService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _storageRoot = _configuration["StorageRoot"] ?? "C:\\FileStorage";
        }

        public async Task<FileMetadata> SaveFileAsync(IFormFile file)
        {
            var metadata = new FileMetadata
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                StoragePath = Path.Combine(_storageRoot, Guid.NewGuid().ToString())
            };

            using (var fileStream = new FileStream(metadata.StoragePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            _context.FileMetadata.Add(metadata);
            await _context.SaveChangesAsync();

            return metadata;
        }

        public async Task<Stream> GetFileAsync(string fileId)
        {
            var metadata = await _context.FileMetadata.FindAsync(Guid.Parse(fileId));
            if (metadata == null) return null;

            return new FileStream(metadata.StoragePath, FileMode.Open, FileAccess.Read);
        }

        public async Task DeleteFileAsync(string fileId)
        {
            var metadata = await _context.FileMetadata.FindAsync(Guid.Parse(fileId));
            if (metadata != null)
            {
                File.Delete(metadata.StoragePath);
                _context.FileMetadata.Remove(metadata);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<FileMetadata> GetMetadataAsync(string fileId)
        {
            return await _context.FileMetadata.FindAsync(Guid.Parse(fileId));
        }
    }
}
