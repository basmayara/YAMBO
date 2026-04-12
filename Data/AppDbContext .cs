using Microsoft.EntityFrameworkCore;
using YamboAPI.Model;
using YamboAPI.Model;

namespace YamboAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<PasswordResetToken>().ToTable("PasswordResetTokens");
        }
    }
}