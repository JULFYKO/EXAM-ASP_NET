namespace EXAM_ASP_NET.Data.Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Product? Product { get; set; }
    }
}
