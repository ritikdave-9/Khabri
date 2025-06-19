using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class NewsSourceTokenConfiguration : IEntityTypeConfiguration<NewsSourceToken>
    {
        public void Configure(EntityTypeBuilder<NewsSourceToken> builder)
        {
            builder.ToTable("NewsSourceTokens");

            builder.HasKey(t => t.NewsSourceTokenID);

            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasOne(t => t.NewsSource)
                .WithMany(ns => ns.NewsSourceTokens)
                .HasForeignKey(t => t.NewsSourceID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
