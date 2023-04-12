using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Models;
using ShoppingApp.Services;

namespace ShoppingApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartService _shoppingCartService;
        private UserManager<ApplicationUser> _userManager;


        public ProductsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
            , IShoppingCartService shoppingCartService)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = (ShoppingCartService)shoppingCartService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Name,Description,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Name,Description,Price")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize]
        public async Task<IActionResult> AddItemToCart(int ProductId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;
            var ShoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(s => s.UserId == userId);
            bool IsItemInCart = false;
            bool IsItemTableEmpty = ShoppingCart.CartItems == null;


            if (ShoppingCart != null && !IsItemTableEmpty)
            {
                IsItemInCart = ShoppingCart.CartItems.Exists(i => i.ProductId == ProductId);
            }
            if (ShoppingCart == null)
            {
                ShoppingCart newCart = new ShoppingCart
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                newCart.CartItems.Add(new CartItem
                {
                    CartItemId = Guid.NewGuid().ToString(),
                    ShoppingCartId = newCart.Id,
                    ProductId = ProductId,
                    Quantity = 1,

                });
                await _context.ShoppingCarts.AddAsync(newCart);
            }
            else if (IsItemInCart == false)
            {
                _context.ShoppingCartItems.Add(new CartItem
                {
                    CartItemId = Guid.NewGuid().ToString(),
                    ShoppingCartId = ShoppingCart.Id,
                    ProductId = ProductId,
                    Quantity = 1,
                });
            }
            else
            {
                ShoppingCart.CartItems.FirstOrDefault(i => i.ProductId == ProductId).Quantity++;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> RemoveItemFromCart(int ProductId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;
            var ShoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(s => s.UserId == userId);
            bool IsItemInCart = false;
            int itemQuantity = 0;
            var selectedItem = ShoppingCart.CartItems.Where(item => item.ProductId == ProductId).SingleOrDefault();

            if (ShoppingCart == null)
            {
                ShoppingCart newCart = new ShoppingCart
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                await _context.ShoppingCarts.AddAsync(newCart);
            }

            if (ShoppingCart != null)
            {
                IsItemInCart = ShoppingCart.CartItems.Exists(i => i.ProductId == ProductId);
                itemQuantity = selectedItem.Quantity;
            }
            else if (IsItemInCart == true && itemQuantity > 1)
            {
                itemQuantity--;
            }
            else
            {
                ShoppingCart.CartItems.Remove(selectedItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
