using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ProductConfiguration : EntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> entity)
        {
            base.Configure(entity);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.OwnsOne(e => e.Image, image =>
            {
                image.Property(i => i.FileName).HasMaxLength(200);
                image.Ignore(i => i.Url);
            });
        }
    }
}