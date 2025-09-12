using System.Diagnostics;
using System.Threading.Tasks;
using EXAM_ASP_NET.Data;
using EXAM_ASP_NET.Data.Entities;
using EXAM_ASP.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EXAM_ASP.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopDbContext _db;

        public HomeController(ILogger<HomeController> logger, ShopDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(12)
                .ToListAsync();

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
