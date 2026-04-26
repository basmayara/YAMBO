namespace YAMBO.ShopService.Dtos
{
    public class PurchaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int NewBalance { get; set; }
        public string ItemName { get; set; }
    }
}
