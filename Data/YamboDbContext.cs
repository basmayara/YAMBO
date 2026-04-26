using Microsoft.EntityFrameworkCore;
using YAMBO.ShopService.Models;

namespace YAMBO.ShopService.Data
{
    public class YamboDbContext : DbContext
    {
        public YamboDbContext(DbContextOptions<YamboDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<PlayerWallet> PlayerWallets { get; set; }
        public DbSet<PlayerInventory> PlayerInventories { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CONFIGURER LE SCHEMA PAR DÉFAUT
            modelBuilder.HasDefaultSchema("inventory_service");

            // Configurer les noms de tables explicitement
            modelBuilder.Entity<ShopItem>().ToTable("shop_items", "inventory_service");
            modelBuilder.Entity<PlayerWallet>().ToTable("player_wallets", "inventory_service");
            modelBuilder.Entity<PlayerInventory>().ToTable("player_inventory", "inventory_service");
            modelBuilder.Entity<TransactionLog>().ToTable("transaction_logs", "inventory_service");

            // Contrainte unique (player_id, item_id)
            modelBuilder.Entity<PlayerInventory>()
                .HasIndex(p => new { p.PlayerId, p.ItemId })
                .IsUnique();

            // Username unique
            modelBuilder.Entity<PlayerWallet>()
                .HasIndex(p => p.Username)
                .IsUnique();

            // Indexes pour performance
            modelBuilder.Entity<PlayerInventory>()
                .HasIndex(p => p.PlayerId);

            modelBuilder.Entity<TransactionLog>()
                .HasIndex(t => new { t.PlayerId, t.Timestamp });
        }
    }
}