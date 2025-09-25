using EXAM_ASP_NET.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EXAM_ASP_NET.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cart;
        public CartController(ICartService cart) => _cart = cart;

        public IActionResult Index()
        {
            var items = _cart.GetProducts();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int id)
        {
            _cart.Add(id);
            TempData["SuccessMessage"] = "Товар додано до корзини.";
            return RedirectToAction("Index", "ActiveLots");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            _cart.Remove(id);
            TempData["SuccessMessage"] = "Товар видалено з корзини.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            _cart.Clear();
            TempData["SuccessMessage"] = "Корзина очищена.";
            return RedirectToAction("Index");
        }
    }
}
