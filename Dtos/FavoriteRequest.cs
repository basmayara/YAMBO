namespace YAMBO.ShopService.Dtos
{
    public class FavoriteRequest
    {
        public int PlayerId { get; set; }
        public int ItemId { get; set; }
        public bool IsFavorite { get; set; }
    }
}
