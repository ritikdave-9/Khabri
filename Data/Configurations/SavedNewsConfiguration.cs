using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entity;

namespace Data.Configurations
{
    public class SavedNewsConfiguration : IEntityTypeConfiguration<SavedNews>
    {
        public void Configure(EntityTypeBuilder<SavedNews> builder)
        {
            builder.ToTable("SavedNews");

            builder.HasKey(sn => new { sn.UserId, sn.NewsId });

            builder.HasOne(sn => sn.User)
                .WithMany()
                .HasForeignKey(sn => sn.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sn => sn.News)
                .WithMany()
                .HasForeignKey(sn => sn.NewsId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(sn => sn.CreatedAt)
                .IsRequired();
        }
    }
}
