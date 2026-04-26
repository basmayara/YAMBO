namespace YAMBO.ShopService.Dtos
{
    public class SellResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RefundAmount { get; set; }
        public int NewBalance { get; set; }
    }
}
