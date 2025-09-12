using System;
using EXAM_ASP_NET.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EXAM_ASP_NET.Data
{
    public class ShopDbContext : IdentityDbContext
    {
        public ShopDbContext()
        {
            //Database.EnsureCreated();
        }
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Auction;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(new List<Category>()
                {
                    new() { Id = 1, Name = "Electronics (Auction)" },
                    new() { Id = 2, Name = "Collectibles" },
                    new() { Id = 3, Name = "Fashion & Wearables" },
                    new() { Id = 4, Name = "Home & Interiors" },
                    new() { Id = 5, Name = "Vehicles & Transport" },
                    new() { Id = 6, Name = "Toys & Hobbies (Rare)" },
                    new() { Id = 7, Name = "Musical Instruments (Vintage)" },
                    new() { Id = 8, Name = "Fine Art & Prints" },
                    new() { Id = 9, Name = "Miscellaneous Lots" }
                });

            modelBuilder.Entity<Product>().HasData(new List<Product>()
                {
                    new()
                    {
                        Id = 1,
                        Title = "Lot 001: iPhone X - Auction Grade B",
                        CategoryId = 1,
                        ImageUrl = "https://applecity.com.ua/image/cache/catalog/0iphone/ipohnex/iphone-x-black-1000x1000.png",
                        StartingPrice = 650m,
                        CurrentBid = null,
                        BuyNowPrice = null,
                        ReservePrice = null,
                        IsAuction = true,
                        AuctionStart = new DateTime(2025, 1, 1),
                        AuctionEnd = new DateTime(2025, 12, 31),
                        Description = "Pre-owned iPhone X sold via auction."
                    },
                    new()
                    {
                        Id = 2,
                        Title = "Lot 002: PowerBall Trainer - Collectible",
                        CategoryId = 2,
                        ImageUrl = "https://http2.mlstatic.com/D_NQ_NP_727192-CBT53879999753_022023-V.jpg",
                        StartingPrice = 45.5m,
                        IsAuction = false,
                        AuctionStart = new DateTime(2024, 1, 1),
                        AuctionEnd = new DateTime(2024, 12, 31),
                        Description = "Collectible PowerBall trainer."
                    },
                    new()
                    {
                        Id = 3,
                        Title = "Lot 003: Nike T-Shirt (Vintage Auction Piece)",
                        CategoryId = 3,
                        ImageUrl = "https://www.seekpng.com/png/detail/316-3168852_nike-air-logo-t-shirt-nike-t-shirt.png",
                        StartingPrice = 189m,
                        IsAuction = true,
                        AuctionStart = new DateTime(2025, 6, 1),
                        AuctionEnd = new DateTime(2025, 6, 15),
                        Description = "Vintage Nike T-Shirt offered at auction."
                    },
                    new()
                    {
                        Id = 4,
                        Title = "Lot 004: Samsung S23 - Auction, New/Sealed",
                        CategoryId = 1,
                        ImageUrl = "https://sota.kh.ua/image/cache/data/Samsung-2/samsung-s23-s23plus-blk-01-700x700.webp",
                        StartingPrice = 1200m,
                        IsAuction = true,
                        AuctionStart = new DateTime(2025, 7, 1),
                        AuctionEnd = new DateTime(2025, 7, 10),
                        Description = "Brand new Samsung S23."
                    },
                    new()
                    {
                        Id = 5,
                        Title = "Lot 005: Signed Air Ball (Collectible)",
                        CategoryId = 6,
                        ImageUrl = "https://cdn.shopify.com/s/files/1/0046/1163/7320/products/69ee701e-e806-4c4d-b804-d53dc1f0e11a_grande.jpg",
                        StartingPrice = 50m,
                        IsAuction = false,
                        AuctionStart = new DateTime(2024, 1, 1),
                        AuctionEnd = new DateTime(2024, 12, 31),
                        Description = "Signed basketball - collectible item."
                    },
                    new()
                    {
                        Id = 6,
                        Title = "Lot 006: MacBook Pro 2019 - Estate Sale",
                        CategoryId = 1,
                        ImageUrl = "https://newtime.ua/image/import/catalog/mac/macbook_pro/MacBook-Pro-16-2019/MacBook-Pro-16-Space-Gray-2019/MacBook-Pro-16-Space-Gray-00.webp",
                        StartingPrice = 900m,
                        IsAuction = false,
                        AuctionStart = new DateTime(2024, 1, 1),
                        AuctionEnd = new DateTime(2024, 12, 31),
                        Description = "MacBook Pro from estate sale."
                    },
                    new()
                    {
                        Id = 7,
                        Title = "Lot 007: Samsung S4 - Vintage Phone Lot",
                        CategoryId = 2,
                        StartingPrice = 440m,
                        IsAuction = false,
                        AuctionStart = new DateTime(2023, 1, 1),
                        AuctionEnd = new DateTime(2023, 12, 31),
                        Description = "Vintage Samsung S4 phone lot."
                    },
                });
        }
    }
}