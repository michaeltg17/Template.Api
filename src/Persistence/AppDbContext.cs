using Microsoft.EntityFrameworkCore;
using Core.Domain;
using Domain.Models;

namespace Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<ImageGroup> ImageGroups { get; set; }
        public virtual DbSet<ImageType> ImageTypes { get; set; }
        public virtual DbSet<ImageResolution> ImageResolutions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ImageFileExtension> ImageFileExtensions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string? migrationConnStr = AppContext.GetData("EfDatabase.MigrationConnectionString") as string
                    ?? Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

                if (!string.IsNullOrEmpty(migrationConnStr))
                    optionsBuilder.UseSqlServer(migrationConnStr, sql => sql.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(builder);
        }

        public Task<int> Delete<T>(long id) where T : class, IIdentifiable
        {
            return Set<T>().Where(e => e.Id == id).ExecuteDeleteAsync();
        }
    }
}
