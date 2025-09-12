using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXAM_ASP_NET.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MinLength(3, ErrorMessage = "Title must has at least 3 characters.")]
        [RegularExpression(@"^[A-Z].*", ErrorMessage = "Title must start with a capital letter.")]
        public string Title { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Range(0.01, 10000000, ErrorMessage = "Starting price must be greater than 0.")]
        public decimal StartingPrice { get; set; }

        [Range(0.01, 10000000, ErrorMessage = "Current bid must be greater than 0.")]
        public decimal? CurrentBid { get; set; }

        [Range(0.01, 10000000, ErrorMessage = "BuyNow price must be greater than 0.")]
        public decimal? BuyNowPrice { get; set; }

        [Range(0.01, 10000000, ErrorMessage = "Reserve price must be greater than 0.")]
        public decimal? ReservePrice { get; set; }

        [Range(0.01, 10000000, ErrorMessage = "Minimum bid increment must be greater than 0.")]
        public decimal MinBidIncrement { get; set; } = 0.01m;
        [NotMapped]
        public decimal BidIncrement
        {
            get => MinBidIncrement;
            set => MinBidIncrement = value;
        }

        [Range(1, 10000000, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        public bool IsAuction { get; set; } = true;

        public DateTime? AuctionStart { get; set; }
        public DateTime? AuctionEnd { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive => DateTime.UtcNow >= AuctionStart && DateTime.UtcNow <= AuctionEnd;

        public int? WinningUserId { get; set; }

        [MinLength(10), MaxLength(3000)]
        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}