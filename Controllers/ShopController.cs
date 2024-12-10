using Microsoft.AspNetCore.Mvc;
using AromaAura.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AromaAura.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string category, string sortOrder, string search)
        {
            // Fetch all products initially
            var products = _context.Products.AsQueryable();

            // Filter by category if provided
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            // Search products by name
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }

            // Sort products by price
            switch (sortOrder)
            {
                case "PriceLowToHigh":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "PriceHighToLow":
                    products = products.OrderByDescending(p => p.Price);
                    break;
            }

            // Get distinct categories for the filter dropdown
            var categories = _context.Products
                                      .Select(p => p.Category)
                                      .Distinct()
                                      .ToList();

            // Pass categories and selected options to the view via ViewData
            ViewData["Categories"] = categories;
            ViewData["SelectedCategory"] = category;
            ViewData["SortOrder"] = sortOrder;

            return View(products.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                                         .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Fetch 4 random products for recommendations
            var randomFeaturedProducts = await _context.Products
                                                       .Where(p => p.Id != id)
                                                       .OrderBy(r => Guid.NewGuid())
                                                       .Take(4)
                                                       .ToListAsync();

            ViewData["FeaturedProducts"] = randomFeaturedProducts;

            return View(product);
        }
    }
}
