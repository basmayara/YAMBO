using MySqlX.XDevAPI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace YAMBO.ShopService.Models
{

    public class ShopItem
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("item_name")]
        public string ItemName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("item_type")]
        public string ItemType { get; set; }
        [Required]
        [Column("price")]
        public int Price { get; set; }
        [MaxLength(200)]
        [Column("description")]
        public string Description { get; set; }

        [MaxLength(200)]
        [Column("sprite_url")]
        public string? SpriteUrl { get; set; }

        [Column("is_global_available")]
        public bool IsGlobalAvailable { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public bool IsAvailable { get; set; } = true;
    }
    
    
}
