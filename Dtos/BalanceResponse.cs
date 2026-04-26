namespace YAMBO.ShopService.Dtos
{
    public class BalanceResponse
    {
        public int PlayerId { get; set; }
        public string Username { get; set; }
        public int Balance { get; set; }
        public int TotalEarned { get; set; }
        public int TotalSpent { get; set; }
    }
}
