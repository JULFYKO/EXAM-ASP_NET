using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EXAM_ASP_NET.Data;
using EXAM_ASP_NET.Data.Entities;

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
            return View("Create", new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
                return View("Create", product);
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
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
            return RedirectToAction(nameof(Index));
        }
    }
}