using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
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

        public async Task<IActionResult> IndexAsync()
        {

            bool IsUserLoggedin = User.Identity.IsAuthenticated;
            ShoppingCart cart = null;

            if (IsUserLoggedin)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string userId = user.Id;
                bool DoesUserHaveCart = await _shoppingCartService.DoesUserHaveCart(userId);

                if (DoesUserHaveCart == false)
                {
                    await _shoppingCartService.CreateShoppingCartAsync(userId);
                }

                cart = await _shoppingCartService.GetShoppingCartAsync(userId);
            }



            var tables = new ProductViewModel
            {
                Products = (List<Models.Product>)_context.Products.Include(p => p.Category),
                ShoppingCart = cart
            };
            return View(tables);
        }
    }
}
