using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ProductConfiguration : EntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            builder.Property(e => e.Price).HasPrecision(18, 2);
            builder.OwnsOne(e => e.Image, image =>
            {
                image.Property(i => i.FileName).HasMaxLength(200);
                image.Ignore(i => i.Url);
            });
        }
    }
}