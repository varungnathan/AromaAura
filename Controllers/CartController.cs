using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AromaAura.Data;
using AromaAura.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AromaAura.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // View the cart (this will display all items added to the cart)
        public async Task<IActionResult> ViewCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the user's cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                // If no cart exists, show the empty cart page
                ViewData["CartItemCount"] = 0;
                return View("EmptyCart");
            }

            // Get the items in the cart and include the associated product details
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .Include(ci => ci.Product)
                .ToListAsync();

            // Set the cart item count to the total number of items
            ViewData["CartItemCount"] = cartItems.Sum(ci => ci.Quantity);

            // Calculate the total cost of the cart
            var totalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price);
            ViewData["TotalAmount"] = totalAmount;

            return View(cartItems);
        }

        // Add an item to the cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if the user already has a cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            // If no cart exists, create one
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Save to get the CartId
            }

            // Check if the product already exists in the cart
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            // If the item already exists in the cart, update the quantity
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                // Otherwise, create a new cart item
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };

                _context.CartItems.Add(cartItem);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the cart page or display a success message
            return RedirectToAction("ViewCart");
        }

        // Remove an item from the cart
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        // Update the quantity of an item in the cart
        [HttpPost]
        public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem != null)
            {
                // Ensure the quantity is valid (greater than 0)
                if (quantity > 0)
                {
                    cartItem.Quantity = quantity;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Quantity must be greater than zero.");
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        // Checkout process (optional, can be expanded later)
        public IActionResult Checkout()
        {
            // Logic to handle the checkout process, for now, just redirect to a checkout page
            return View();
        }
    }
}
