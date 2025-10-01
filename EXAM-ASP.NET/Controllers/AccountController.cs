using EXAM_ASP_NET.Data;
using EXAM_ASP_NET.Models;
using EXAM_ASP_NET.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace EXAM_ASP_NET.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ShopDbContext _db;
        private readonly IAppEmailSender _emailSender;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ShopDbContext db, IAppEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName) ?? await _userManager.FindByEmailAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (model.SendCredentials && !string.IsNullOrEmpty(model.Email))
                {
                    try
                    {
                        var subject = "Your account credentials";
                        var html = $"<p>Дякуємо за реєстрацію.</p><p>Логін: <strong>{model.UserName}</strong></p><p>Пароль: <strong>{model.Password}</strong></p>";
                        await _emailSender.SendEmailAsync(model.Email, subject, html);
                        TempData["SuccessMessage"] = "Реєстрація пройшла успішно. Лист із даними відправлено.";
                    }
                    catch
                    {
                        TempData["ErrorMessage"] = "Користувача створено, але не вдалося відправити лист. Перевірте налаштування SMTP.";
                    }
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var e in result.Errors)
            {
                ModelState.AddModelError(string.Empty, e.Description);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login");

            var currentName = User.Identity?.Name;

            var bids = await _db.Bids
                .Where(b => b.UserId == uid || (currentName != null && b.UserId == currentName))
                .Include(b => b.Product)
                .OrderByDescending(b => b.Timestamp)
                .ToListAsync();

            return View(bids);
        }
    }
}
