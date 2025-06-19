using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class UserKeywordConfiguration : IEntityTypeConfiguration<UserKeyword>
    {
        public void Configure(EntityTypeBuilder<UserKeyword> builder)
        {
            builder.ToTable("UserKeywords");

            builder.HasKey(uk => new { uk.UserID, uk.KeywordID });

            builder.HasOne(uk => uk.User)
                .WithMany()
                .HasForeignKey(uk => uk.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uk => uk.Keyword)
                .WithMany()
                .HasForeignKey(uk => uk.KeywordID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(uk => uk.CreatedAt)
                .IsRequired();
        }
    }
}
