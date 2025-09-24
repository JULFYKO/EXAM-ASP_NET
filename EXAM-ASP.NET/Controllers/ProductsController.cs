using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EXAM_ASP_NET.Data;
using EXAM_ASP_NET.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EXAM_ASP_NET.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopDbContext _db;
        public ProductsController(ShopDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var items = await _db.Products.Include(p => p.Category).ToListAsync();
            return View(items);
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

            var exists = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (exists == null) return NotFound();

            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Лот успішно оновлено.";
            return RedirectToAction("Index", "ActiveLots");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Лот успішно видалено.";
            return RedirectToAction("Index", "ActiveLots");
        }
    }
}
