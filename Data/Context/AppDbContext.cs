using Microsoft.EntityFrameworkCore;
using Data.Entity;
using Data.Configurations;
using Data.Configuration;

namespace Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<NewsCategory> NewsCategories { get; set; }
        public DbSet<NewsKeyword> NewsKeywords { get; set; }
        public DbSet<NewsSource> NewsSources { get; set; }
        public DbSet<NewsSourceToken> NewsSourceTokens { get; set; }
        public DbSet<SavedNews> SavedNews { get; set; }
        public DbSet<UserKeyword> UserKeywords { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserKeywordConfiguration());
            modelBuilder.ApplyConfiguration(new SavedNewsConfiguration());
            modelBuilder.ApplyConfiguration(new NewsSourceTokenConfiguration());
            modelBuilder.ApplyConfiguration(new NewsSourceConfiguration());
            modelBuilder.ApplyConfiguration(new NewsKeywordConfiguration());
            modelBuilder.ApplyConfiguration(new NewsCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new KeywordConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new NewsConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
