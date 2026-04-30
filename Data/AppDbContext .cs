using Microsoft.EntityFrameworkCore;
using YamboAPI.Model;

namespace YamboAPI.Data
{
    
    public class AppDbContext : DbContext
    {
        // constructor injection: DbContextOptions contains the connection string and provider
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        //  DbSet = represents a table in the database
        // EF Core uses these for queries and change tracking
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicit table name mapping — keeps DB table names clean and predictable
            // "User" table for User model, "PasswordResetTokens" for PasswordResetToken model
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<PasswordResetToken>().ToTable("PasswordResetTokens");
        }
    }
}