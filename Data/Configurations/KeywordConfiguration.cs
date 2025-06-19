using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class KeywordConfiguration : IEntityTypeConfiguration<Keyword>
    {
        public void Configure(EntityTypeBuilder<Keyword> builder)
        {
            builder.ToTable("Keywords");

            builder.HasKey(k => k.KeywordID);

            builder.Property(k => k.KeywordText)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(k => k.CreatedAt)
                .IsRequired();
        }
    }
}
