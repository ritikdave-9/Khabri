using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class NewsKeywordConfiguration : IEntityTypeConfiguration<NewsKeyword>
    {
        public void Configure(EntityTypeBuilder<NewsKeyword> builder)
        {
            builder.ToTable("NewsKeywords");

            builder.HasKey(nk => new { nk.NewsID, nk.KeywordID });

            builder.HasOne(nk => nk.News)
                .WithMany()
                .HasForeignKey(nk => nk.NewsID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(nk => nk.Keyword)
                .WithMany()
                .HasForeignKey(nk => nk.KeywordID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(nk => nk.CreatedAt)
                .IsRequired();
        }
    }
}
