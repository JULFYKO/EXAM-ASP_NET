using EXAM_ASP_NET.Data;
using EXAM_ASP_NET.Data.Entities;
using EXAM_ASP_NET.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EXAM_ASP_NET.Services
{
    public class CookieCartService : ICartService
    {
        private const string CookieName = "CartItems";
        private readonly IHttpContextAccessor _http;
        private readonly ShopDbContext _db;

        public CookieCartService(IHttpContextAccessor http, ShopDbContext db)
        {
            _http = http;
            _db = db;
        }

        public List<int> GetItemIds()
        {
            var ctx = _http.HttpContext;
            if (ctx == null) return new List<int>();
            if (!ctx.Request.Cookies.TryGetValue(CookieName, out var value) || string.IsNullOrWhiteSpace(value))
                return new List<int>();

            try
            {
                var ids = JsonSerializer.Deserialize<List<int>>(value);
                return ids ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        public List<Product> GetProducts()
        {
            var ids = GetItemIds();
            if (ids == null || ids.Count == 0) return new List<Product>();
            var products = _db.Products.Where(p => ids.Contains(p.Id)).ToList();
            // preserve order as in ids
            var ordered = ids.Distinct().Select(id => products.FirstOrDefault(p => p.Id == id)).Where(p => p != null).Cast<Product>().ToList();
            return ordered;
        }

        private void SaveIds(List<int> ids)
        {
            var ctx = _http.HttpContext;
            if (ctx == null) return;
            var json = JsonSerializer.Serialize(ids);
            ctx.Response.Cookies.Append(CookieName, json, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true
            });
        }

        public void Add(int id)
        {
            var ids = GetItemIds();
            ids.Add(id);
            // keep small, distinct
            ids = ids.Where(i => i > 0).ToList();
            SaveIds(ids);
        }

        public void Clear()
        {
            var ctx = _http.HttpContext;
            if (ctx == null) return;
            ctx.Response.Cookies.Delete(CookieName);
        }

        public int GetCartSize()
        {
            return GetItemIds().Count;
        }
    }
}
