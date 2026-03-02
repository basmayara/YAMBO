using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using YAMBO.Model;
using YAMBO.Model.YAMBO.Model;

namespace YAMBO.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User"); //  Map l table "User" f DB
        }
    }
}