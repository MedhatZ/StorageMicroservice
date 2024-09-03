// Models/FileMetadata.cs
using System;

namespace StorageMicroservice.Models
{
    public class FileMetadata
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public string StoragePath { get; set; }
    }
}
