namespace YAMBO.ShopService.Dtos
{
    public class InventoryItemDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int RefundPrice => (int)(Price * 0.5f);
        public string IconUrl { get; set; } = string.Empty;
        public DateTime AcquireAt { get; set; }
        public bool IsFavorite { get; set; }

    }
}
