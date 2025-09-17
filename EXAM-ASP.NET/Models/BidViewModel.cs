using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EXAM_ASP_NET.Models
{
    public class BidDto
    {
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class BidViewModel
    {
        public int ProductId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }

        public decimal StartingPrice { get; set; }
        public decimal? CurrentBid { get; set; }
        public decimal? BuyNowPrice { get; set; }
        public decimal MinBidIncrement { get; set; } = 0.01m;
        public DateTime? AuctionStart { get; set; }
        public DateTime? AuctionEnd { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Your bid")]
        public decimal BidAmount { get; set; }

        // History
        public List<BidDto>? BidHistory { get; set; }

        // Messages to show after POST
        public string? Message { get; set; }
    }
}
