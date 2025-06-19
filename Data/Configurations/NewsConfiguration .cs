using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.ToTable("News");

            builder.HasKey(n => n.NewsID);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.Description)
                .HasMaxLength(2000);

            builder.Property(n => n.Url)
                .HasMaxLength(1000);

            builder.Property(n => n.ImageUrl)
                .HasMaxLength(1000);

            builder.Property(n => n.Content)
                .HasColumnType("TEXT");  

            builder.Property(n => n.Source)
                .HasMaxLength(200);

            builder.Property(n => n.Author)
                .HasMaxLength(200);

            builder.Property(n => n.PublishedAt)
                .IsRequired();

            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.Property(n => n.Language)
                .HasMaxLength(10);

            builder.HasOne<NewsSource>()
                   .WithMany()
                   .HasForeignKey(n => n.NewsSourceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
