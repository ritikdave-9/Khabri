using Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<NewsSource> NewsSources { get; set; }
        public DbSet<NewsSourceToken> NewsSourceTokens { get; set; }
        public DbSet<NewsSourceMappingField> NewsSourceMappingFields { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<NewsLikeDislike> NewsLikeDislikes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.UserID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<News>()
                .Property(n => n.NewsID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Keyword>()
                .Property(k => k.KeywordID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<NewsSource>()
                .Property(ns => ns.NewsSourceID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<NewsSourceToken>()
                .Property(nst => nst.NewsSourceTokenID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<NewsSourceMappingField>()
                .Property(nsmf => nsmf.NewsSourceMappingFieldID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserSubscription>()
                .Property(us => us.UserSubscriptionID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Notification>()
                .Property(n => n.NotificationID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Report>()
                .Property(r => r.ReportID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<NewsLikeDislike>()
                .Property(nld => nld.NewsLikeDislikeID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<NewsSource>()
                .HasIndex(ns => ns.BaseURL)
                .IsUnique();

            modelBuilder.Entity<News>()
                .HasIndex(n => n.Url)
                .IsUnique();

            modelBuilder.Entity<NewsLikeDislike>()
                .HasIndex(nld => new { nld.UserID, nld.NewsID })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
