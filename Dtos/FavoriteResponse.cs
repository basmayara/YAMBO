namespace YAMBO.ShopService.Dtos
{
    public class FavoriteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
    }
}
