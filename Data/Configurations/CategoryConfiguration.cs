using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categorie");

            builder.HasKey(c => c.CategoryID);

            builder.Property(c => c.CategoryName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.IsActive)
                .IsRequired();
        }
    }
}
