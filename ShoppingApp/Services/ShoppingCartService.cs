using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingApp.Data;
using ShoppingApp.Models;

namespace ShoppingApp.Services
{

    public interface IShoppingCartService
    {
        Task<ShoppingCart> CreateShoppingCartAsync(string UserId);
        Task AddItem(string UserId, int ProductId);
        Task RemoveItem(string UserId, int ProductId);
        Task<ShoppingCart> GetShoppingCartAsync(string id);
        int CountItemsInCart(ShoppingCart cart);
        decimal TotalPriceOfCart(ShoppingCart cart);
        Task ClearCart(string UserId, List<CartItem> c);

        Task<bool> DoesUserHaveCart(string UserId);

    }
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;


        public ShoppingCartService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        /// <summary>
        /// Creates a cart for a user
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>ShoppingCart</returns>
        public async Task<ShoppingCart> CreateShoppingCartAsync(string UserId)
        {
            var newCart = new ShoppingCart
            {
                Id = Guid.NewGuid().ToString(),
                UserId = UserId,
            };
            _context.ShoppingCarts.Add(newCart);

            await _context.SaveChangesAsync();

            return newCart;
        }

        /// <summary>
        /// Returns the cart that matches the user's Id 
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns>ShoppingCart</returns>
        public async Task<ShoppingCart> GetShoppingCartAsync(string UserId)
        {
            var cart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(s => s.UserId == UserId);
            if (cart == null)
            {
                cart = await CreateShoppingCartAsync(UserId);
            }
            return cart;
        }

        /// <summary>
        /// Adds item to user's cart or increases quantity
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ProductId"></param>
        /// <returns>Task</returns>
        public async Task AddItem(string UserId, int ProductId)
        {
            var cart = await GetShoppingCartAsync(UserId);
            Product product = await _context.Products
                .Where(p => p != null && p.Id == ProductId)
                .FirstOrDefaultAsync();
            var IsItemInCart = false;
            bool IsItemTableEmpty = _context.ShoppingCartItems.IsNullOrEmpty();

            if (!IsItemTableEmpty)
            {
                IsItemInCart = _context.ShoppingCartItems
                    .Any(i => i.ShoppingCartId == cart.Id && i.ProductId == ProductId);
            }

            if (IsItemInCart)
            {
                cart.CartItems.FirstOrDefault(i => i.ProductId == ProductId).Quantity++;
            }
            else if (!IsItemInCart)
            {
                var newItem = new CartItem
                {
                    CartItemId = Guid.NewGuid().ToString(),
                    ShoppingCartId = cart.Id,
                    ProductId = ProductId,
                    Quantity = 1,
                    ImgUrl = product.ImgUrl,
                    Name = product.Name,
                    UnitPrice = product.Price,
                };
                _context.ShoppingCartItems.Add(newItem);
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Decreases item's quantity by one or remove if new quantity is 0
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ProductId"></param>
        /// <returns>Task</returns>
        public async Task RemoveItem(string UserId, int ProductId)
        {
            var cart = await GetShoppingCartAsync(UserId);

            var itemInCart = _context.ShoppingCartItems
                .FirstOrDefault(i => i.ShoppingCartId == cart.Id && i.ProductId == ProductId);

            if (itemInCart == null)
            {
                return;
            }

            if (itemInCart.Quantity > 1)
            {
                itemInCart.Quantity--;
            }
            else
            {
                cart.CartItems.Remove(itemInCart);
            };
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes all items from the user's cart
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="cartItems"></param>
        /// <returns>Task</returns>
        public async Task ClearCart(string UserId, List<CartItem> cartItems)
        {
            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Check if user has a cart
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<bool> DoesUserHaveCart(string UserId)
        {
            bool doesUserHaveCart = false;
            var userCart = await _context.ShoppingCarts.FirstOrDefaultAsync(s => s.UserId == UserId);
            if (userCart != null)
            {
                doesUserHaveCart = true;
            }
            return doesUserHaveCart;
        }

        /// <summary>
        /// Returns amount of items in user's cart
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public int CountItemsInCart(ShoppingCart cart)
        {
            List<CartItem> ItemsInCart = _context.ShoppingCartItems.Where(item => item.ShoppingCartId == cart.Id).ToList();
            int count = 0;
            foreach (var item in ItemsInCart)
            {
                count += item.Quantity;
            }

            return count;
        }
        /// <summary>
        /// Returns sum of all item's UnitPrice * Quantity
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public decimal TotalPriceOfCart(ShoppingCart cart)
        {
            bool IsCartEmpty = CountItemsInCart(cart) == 0;
            if (!IsCartEmpty)
            {
                List<CartItem> ItemsInCart = _context.ShoppingCartItems.Where(item => item != null && item.ShoppingCartId == cart.Id).ToList();
                decimal total = 0;

                foreach (var item in ItemsInCart)
                {
                    total += item.UnitPrice * item.Quantity;
                }

                return total;
            }
            else { return 0; }
        }
    }
}
