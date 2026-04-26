namespace YAMBO.ShopService.Dtos
{
    public class ShopItemResponse
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string SpriteUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
