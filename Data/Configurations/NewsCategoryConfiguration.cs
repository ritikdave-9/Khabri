using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class NewsCategoryConfiguration : IEntityTypeConfiguration<NewsCategory>
    {
        public void Configure(EntityTypeBuilder<NewsCategory> builder)
        {
            builder.ToTable("NewsCategories");

            builder.HasKey(nc => new { nc.NewsID, nc.CategoryID });

            builder.HasOne(nc => nc.News)
                .WithMany() 
                .HasForeignKey(nc => nc.NewsID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(nc => nc.Category)
                .WithMany() 
                .HasForeignKey(nc => nc.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(nc => nc.CreatedAt)
                .IsRequired();
        }
    }
}
