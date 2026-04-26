using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YAMBO.ShopService.Models
{
    [Table("transaction_logs", Schema = "inventory_service")]
    public class TransactionLog
    {
        [Key]
        [Column("transaction_id")]
        public int TransactionId { get; set; }

        [Required]
        [Column("player_id")]
        public int PlayerId { get; set; }

        [Column("item_id")]
        public int? ItemId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("transaction_type")]
        public string TransactionType { get; set; } // "purchase", "reward", "refund"

        [Required]
        [Column("amount")]
        public int Amount { get; set; }

        [Column("balance_before")]
        public int BalanceBefore { get; set; }

        [Column("balance_after")]
        public int BalanceAfter { get; set; }

        [MaxLength(45)]
        [Column("ip_address")]
        public string IpAddress { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("PlayerId")]
        public PlayerWallet Player { get; set; }

        [ForeignKey("ItemId")]
        public ShopItem Item { get; set; }
    }
}
