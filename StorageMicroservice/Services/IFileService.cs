// Services/IFileService.cs
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StorageMicroservice.Models;

namespace StorageMicroservice.Services
{
    public interface IFileService
    {
        Task<FileMetadata> SaveFileAsync(IFormFile file);
        Task<Stream> GetFileAsync(string fileId);
        Task DeleteFileAsync(string fileId);
        Task<FileMetadata> GetMetadataAsync(string fileId);
    }
}
