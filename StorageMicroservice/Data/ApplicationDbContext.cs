// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using StorageMicroservice.Models;
using System.Collections.Generic;

namespace StorageMicroservice.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FileMetadata> FileMetadata { get; set; }
    }
}
