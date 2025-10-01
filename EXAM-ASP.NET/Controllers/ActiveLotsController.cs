using EXAM_ASP_NET.Data;
using EXAM_ASP_NET.Data.Entities;
using EXAM_ASP_NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace EXAM_ASP_NET.Controllers
{
    public class ActiveLotsController : Controller
    {
        private readonly ShopDbContext _db;
        public ActiveLotsController(ShopDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var items = await _db.Products.Include(p => p.Category).ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name");
            return View("~/Views/Products/Create.cshtml", new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
                return View("~/Views/Products/Create.cshtml", product);
            }

            if (product.AuctionStart == null)
            {
                product.AuctionStart = product.StartDate ?? DateTime.UtcNow;
            }

            if (product.AuctionEnd == null)
            {
                product.AuctionEnd = product.EndDate ?? product.AuctionStart?.AddDays(1) ?? DateTime.UtcNow.AddDays(1);
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Лот успішно створено.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
                return View(product);
            }

            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Лот успішно оновлено.";
            return RedirectToAction(nameof(Index));
        }
    
    public async Task<IActionResult> Bid(int id)
        {
            var product = await _db.Products
        .Include(p => p.Category)
    .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }


            var vm = new BidViewModel
            {
                ProductId = product.Id,
                Title = product.Title,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name,
                StartingPrice = product.StartingPrice,
                CurrentBid = product.CurrentBid,
                BuyNowPrice = product.BuyNowPrice,
                MinBidIncrement = product.MinBidIncrement > 0 ? product.MinBidIncrement : 0.01m,
                AuctionStart = product.AuctionStart,
                AuctionEnd = product.AuctionEnd,
                Quantity = product.Quantity,
                Description = product.Description,
                BidAmount = Math.Max(product.CurrentBid ?? product.StartingPrice, product.StartingPrice) + (product.MinBidIncrement > 0 ? product.MinBidIncrement : 0.01m),
                BidHistory = await _db.Bids
                              .Where(b => b.ProductId == id)
                              .OrderByDescending(b => b.Timestamp)
                              .Select(b => new BidDto { UserId = b.UserId, Amount = b.Amount, Timestamp = b.Timestamp })
                              .ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Bid(BidViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == model.ProductId && p.IsAuction);
            if (product == null) return NotFound();

            var nowUtc = DateTime.UtcNow;
            if (product.AuctionStart.HasValue && product.AuctionStart.Value.ToUniversalTime() > nowUtc)
            {
                model.Message = "Аукціон ще не почався.";
                model.BidHistory = await _db.Bids.Where(b => b.ProductId == model.ProductId)
                                                .OrderByDescending(b => b.Timestamp)
                                                .Select(b => new BidDto { UserId = b.UserId, Amount = b.Amount, Timestamp = b.Timestamp })
                                                .ToListAsync();
                return View(model);
            }
            if (product.AuctionEnd.HasValue && product.AuctionEnd.Value.ToUniversalTime() < nowUtc)
            {
                model.Message = "Аукціон вже закінчився.";
                model.BidHistory = await _db.Bids.Where(b => b.ProductId == model.ProductId)
                                                .OrderByDescending(b => b.Timestamp)
                                                .Select(b => new BidDto { UserId = b.UserId, Amount = b.Amount, Timestamp = b.Timestamp })
                                                .ToListAsync();
                return View(model);
            }

            var currentTop = product.CurrentBid ?? product.StartingPrice;
            var minAllowed = currentTop + (product.MinBidIncrement > 0 ? product.MinBidIncrement : 0.01m);
            if (model.BidAmount < minAllowed)
            {
                ModelState.AddModelError(nameof(model.BidAmount), $"Ставка має бути не менше {minAllowed:C}.");
                model.Message = "Занадто мала сума ставки.";
                model.BidHistory = await _db.Bids.Where(b => b.ProductId == model.ProductId)
                                                .OrderByDescending(b => b.Timestamp)
                                                .Select(b => new BidDto { UserId = b.UserId, Amount = b.Amount, Timestamp = b.Timestamp })
                                                .ToListAsync();
                return View(model);
            }

            if (product.BuyNowPrice.HasValue && model.BidAmount >= product.BuyNowPrice.Value)
            {
                product.CurrentBid = product.BuyNowPrice;
                product.IsAuction = false;
                product.WinningUserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? (int?)uid : null;

                var buyBid = new Bid
                {
                    ProductId = product.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "unknown",
                    Amount = product.BuyNowPrice.Value,
                    Timestamp = DateTime.UtcNow
                };
                _db.Bids.Add(buyBid);

                await _db.SaveChangesAsync();

                model.Message = "Товар викуплено по Buy Now.";
                model.CurrentBid = product.CurrentBid;
                model.BidHistory = await _db.Bids.Where(b => b.ProductId == model.ProductId)
                                                .OrderByDescending(b => b.Timestamp)
                                                .Select(b => new BidDto { UserId = b.UserId, Amount = b.Amount, Timestamp = b.Timestamp })
                                                .ToListAsync();
                return View(model);
            }

            var bidRecord = new Bid
            {
                ProductId = product.Id,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "unknown",
                Amount = model.BidAmount,
                Timestamp = DateTime.UtcNow
            };
            _db.Bids.Add(bidRecord);

            product.CurrentBid = model.BidAmount;

            await _db.SaveChangesAsync();

            model.CurrentBid = product.CurrentBid;
            model.Message = "Ставка прийнята.";
            model.BidHistory = await _db.Bids.Where(b => b.ProductId == model.ProductId)
                                            .OrderByDescending(b => b.Timestamp)
                                            .Select(b => new BidDto { UserId = b.UserId, Amount = b.Amount, Timestamp = b.Timestamp })
                                            .ToListAsync();

            return View(model);
        }
    }
}