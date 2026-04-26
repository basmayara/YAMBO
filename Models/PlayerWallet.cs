using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YAMBO.ShopService.Models
{
    [Table("player_wallets", Schema = "inventory_service")]
    public class PlayerWallet
    {
        [Key]
        [Column("player_id")]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; set; }
        [Column("balance")]
        public int Balance { get; set; } = 1000;
        [Column("total_earned")]
        public int TotalEarned { get; set; } = 1000;
        [Column("total_spent")]
        public int TotalSpent { get; set; } = 0;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
