using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class NewsSourceConfiguration : IEntityTypeConfiguration<NewsSource>
    {
        public void Configure(EntityTypeBuilder<NewsSource> builder)
        {
            builder.ToTable("NewsSources");

            builder.HasKey(ns => ns.NewsSourceID);

            builder.Property(ns => ns.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ns => ns.BaseURL)
                .HasMaxLength(500);

            builder.Property(ns => ns.CreatedAt)
                .IsRequired();

            builder.HasMany(ns => ns.NewsSourceTokens)
                   .WithOne(token => token.NewsSource)
                   .HasForeignKey(token => token.NewsSourceID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
