using System.ComponentModel.DataAnnotations;

namespace YAMBO.ShopService.Dtos
{
    public class PurchaseRequest
    {
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public int ItemId { get; set; }
    }
}
