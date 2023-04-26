using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingApp.Data;
using ShoppingApp.Helper;
using ShoppingApp.Models;
using ShoppingApp.Services;
using ShoppingApp.ViewModels;
namespace ShoppingApp.Controllers
{
    public class ProductViewModelController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartService _shoppingCartService;
        private UserManager<ApplicationUser> _userManager;

        public ProductViewModelController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
            , IShoppingCartService shoppingCartService)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = (ShoppingCartService)shoppingCartService;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId, int? page)
        {

            bool IsUserLoggedin = User.Identity.IsAuthenticated;
            ShoppingCart? cart = null;

            if (IsUserLoggedin)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string userId = user.Id;
                bool DoesUserHaveCart = await _shoppingCartService.DoesUserHaveCart(userId);

                if (DoesUserHaveCart == false)
                {
                    cart = await _shoppingCartService.CreateShoppingCartAsync(userId);
                }
                else
                {
                    cart = await _shoppingCartService.GetShoppingCartAsync(userId);
                }


            }
            var productList = _context.Products.Select(s => s);

            if (!String.IsNullOrEmpty(searchString))
            {
                productList = productList.Where(p => p.Name.Contains(searchString));
            }

            var categoryList = _context.Categories.Select(c => c);
            var categoryDictionary = categoryList
                .ToDictionary(k => k.Id, v => v.Name);
            ViewData["CategorySelectList"] = new SelectList(categoryDictionary, "Key", "Value");

            if (categoryId != null)
            {
                productList = productList.Where(c => c.CategoryId == categoryId);
            }

            var cartItems = await _context.ShoppingCartItems.ToListAsync();

            int pageSize = 6;
            int pageNumber = page ?? 1;

            var pagedProductList = new PagedList<Product>(productList.ToList(), productList.Count(), pageNumber, pageSize);
            var displayedProductList = PagedList<Product>.ToPagedList(productList, pageNumber, pageSize);

            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            ViewBag.TotalPages = pagedProductList.TotalPages;
            ViewBag.HasNext = pagedProductList.HasNext;
            ViewBag.HasPrevious = pagedProductList.HasPrevious;

            var tables = new ProductViewModel
            {
                Products = displayedProductList,
                Categories = await categoryList.ToListAsync(),
                ShoppingCart = cart,
                CartItems = cartItems

            };
            return await Task.Run(() => View(tables));
        }

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

        [Authorize]
        public async Task<IActionResult> AddItemToCart(int ProductId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;
            var ShoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(s => s.UserId == userId);
            bool IsItemInCart = false;
            bool IsItemTableEmpty = _context.ShoppingCartItems.IsNullOrEmpty();
            Product product = await _context.Products.Where(p => p != null && p.Id == ProductId).FirstOrDefaultAsync();

            if (ShoppingCart != null && !IsItemTableEmpty)
            {
                IsItemInCart = _context.ShoppingCartItems.Any(i => i.ProductId == ProductId);
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
                    Name = product.Name,
                    ImgUrl = product.ImgUrl,
                    UnitPrice = product.Price,

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
                    Name = product.Name,
                    ImgUrl = product.ImgUrl,
                    UnitPrice = product.Price,
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
            var selectedItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(c => c.ProductId == ProductId);

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
            if (IsItemInCart == true && itemQuantity > 1)
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
