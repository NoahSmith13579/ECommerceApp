using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Models;

namespace ShoppingApp.Services
{

    public interface IShoppingCartService
    {
        Task<ShoppingCart> CreateShoppingCartAsync(string UserId);
        void AddItem(string UserId, int ProductId);
        void RemoveItem(string UserId, int ProductId);
        Task<ShoppingCart> GetShoppingCartAsync(string id);
        int CountItemsInCart(ShoppingCart cart);
        decimal TotalPriceOfCart(ShoppingCart cart);
        void ClearCart(string UserId);

        Task<bool> DoesUserHaveCart(string UserId);

    }
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ShoppingCart> CreateShoppingCartAsync(string UserId)
        {
            var newCart = new ShoppingCart
            {
                Id = Guid.NewGuid().ToString(),
                UserId = UserId,
                CartItems = new List<CartItem>()
            };
            _context.ShoppingCarts.Add(newCart);

            await _context.SaveChangesAsync();

            return newCart;
        }

        /// <summary>
        /// Returns the <c>cart</c>  that matches the <c> User's Id </c>
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns></returns>
        public async Task<ShoppingCart> GetShoppingCartAsync(string Userid)
        {
            var cart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(s => s.UserId == Userid);

            return cart;
        }

        public async void AddItem(string UserId, int ProductId)
        {

            var cart = await GetShoppingCartAsync(UserId);
            var itemInCart = cart.CartItems
                .FirstOrDefault(i => i.ProductId == ProductId);

            if (itemInCart != null)
            {
                itemInCart.Quantity++;
            }
            else
            {
                var newItem = new CartItem
                {
                    ShoppingCartId = cart.Id,
                    ProductId = ProductId,
                    Quantity = 1,
                };
                cart.CartItems.Add(newItem);

            }
            await _context.SaveChangesAsync();



        }

        public async void RemoveItem(string UserId, int ProductId)
        {
            var cart = await GetShoppingCartAsync(UserId);

            var itemInCart = cart.CartItems
                .FirstOrDefault(i => i.ProductId == ProductId);
            if (itemInCart != null)
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

        public async void ClearCart(string UserId)
        {
            var cart = await GetShoppingCartAsync(UserId);
            var cartItems = await _context.ShoppingCartItems.Where(item => item.ShoppingCartId == cart.Id).ToListAsync();
            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

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

        public decimal TotalPriceOfCart(ShoppingCart cart)
        {
            List<CartItem> ItemsInCart = cart.CartItems.Where(item => item.ShoppingCartId == cart.Id).ToList();
            decimal total = 0;

            foreach (var item in ItemsInCart)
            {
                Product product = _context.Products.Where(p => p.Id == item.ProductId).First();
                total += product.Price;
            }

            return total;
        }
    }
}
