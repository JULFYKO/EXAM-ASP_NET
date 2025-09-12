namespace EXAM_ASP.NET.Data.Entities
{
    public class Auction
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal StartingPrice { get; set; }
        public decimal? BuyNowPrice { get; set; }
        public decimal MinBidIncrement { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }
}