using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YAMBO.ShopService.Models
{
    [Table("player_inventory", Schema = "inventory_service")]
    public class PlayerInventory
    {
        [Key]
        [Column("inventory_id")]
        public int InventoryId { get; set; }

        [Required]
        [Column("player_id")]
        public int PlayerId { get; set; }

        [Required]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("acquired_at")]
        public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;

        [Column("is_equipped")]
        public bool IsEquipped { get; set; } = false;
        [Column("is_favorite")]
public bool IsFavorite { get; set; } = false;

        [ForeignKey("PlayerId")]
        public PlayerWallet Player { get; set; }

        [ForeignKey("ItemId")]
        public ShopItem? Item { get; set; }

        
    }
}
