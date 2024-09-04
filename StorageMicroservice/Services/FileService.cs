using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StorageMicroservice.Models;
using StorageMicroservice.Data;
using static System.Ulid;
using Microsoft.EntityFrameworkCore;

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
            // Generate a unique ID for the file
            var uniqueId = Ulid.NewUlid().ToString();

            // Get the current date for folder structure
            var now = DateTime.UtcNow;
            var folderPath = Path.Combine(
                _storageRoot,
                now.ToString("yyyy"),
                now.ToString("MM"),
                now.ToString("dd")
            );

            // Ensure the directory exists
            Directory.CreateDirectory(folderPath);

            // Combine the folder path with the unique ID
            var storagePath = Path.Combine(folderPath, uniqueId);

            var metadata = new FileMetadata
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                StoragePath = storagePath
            };

            using (var fileStream = new FileStream(storagePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            _context.FileMetadata.Add(metadata);
            await _context.SaveChangesAsync();

            return metadata;
        }

        public async Task<Stream> GetFileAsync(string fileId)
        {
            try
            {
                // Assuming fileId is a ULID string, you might need to adjust this logic
                var metadata = await _context.FileMetadata
                    .FirstOrDefaultAsync(m => m.StoragePath.Contains(fileId));

                if (metadata == null || !File.Exists(metadata.StoragePath))
                    return null;

                return new FileStream(metadata.StoragePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary
                throw new Exception("Error retrieving file", ex);
            }
        }

        public async Task DeleteFileAsync(string fileId)
        {
            try
            {
                // Check if the fileId is null or empty
                if (string.IsNullOrWhiteSpace(fileId))
                {
                    throw new ArgumentException("File ID cannot be null or empty.", nameof(fileId));
                }

                // Find the metadata based on the fileId, assuming it's stored in StoragePath
                var metadata = await _context.FileMetadata
                    .FirstOrDefaultAsync(m => m.StoragePath.Contains(fileId));

                if (metadata != null)
                {
                    // Delete the file from storage
                    if (File.Exists(metadata.StoragePath))
                    {
                        File.Delete(metadata.StoragePath);
                    }

                    // Remove metadata from the database
                    _context.FileMetadata.Remove(metadata);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary
                throw new Exception("Error deleting file", ex);
            }
        }

        public async Task<FileMetadata> GetMetadataAsync(string fileId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(fileId))
            {
                throw new ArgumentException("File ID cannot be null or empty.", nameof(fileId));
            }

            // Retrieve the metadata based on the fileId
            var metadata = await _context.FileMetadata
                .FirstOrDefaultAsync(m => m.StoragePath.Contains(fileId));

            return metadata; // This will return null if not found
        }
    }
}