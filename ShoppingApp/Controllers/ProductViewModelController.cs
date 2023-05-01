using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Get Index ProductViewModel view
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="categoryId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int? categoryId, int? page)
        {

            ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;
            ShoppingCart cart = await _shoppingCartService.GetShoppingCartAsync(userId);
            var productList = _context.Products.Select(s => s);

            if (!String.IsNullOrEmpty(searchString))
            {
                productList = productList.Where(p => p.Name.Contains(searchString));
            }

            var categoryList = _context.Categories.Select(c => c);
            categoryList = categoryList.OrderBy(c => c.Id == categoryId - 1);
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

        /// <summary>
        /// Adds item to the user's cart
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns>Index view</returns>
        [Authorize]
        public async Task<IActionResult> AddItemToCart(int ProductId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;
            await _shoppingCartService.AddItem(userId, ProductId);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Removes item from the user's cart
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns>Index view</returns>
        [Authorize]
        public async Task<IActionResult> RemoveItemFromCart(int ProductId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;
            await _shoppingCartService.RemoveItem(userId, ProductId);
            return RedirectToAction(nameof(Index));
        }
    }
}
