﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Models;
using ShoppingApp.Services;
using ShoppingApp.ViewModels;

namespace ShoppingApp.Controllers
{
    public class OrderViewModelController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartService _shoppingCartService;
        private UserManager<ApplicationUser> _userManager;

        public OrderViewModelController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
            , IShoppingCartService shoppingCartService)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = (ShoppingCartService)shoppingCartService;

        }
        /// <summary>
        /// Get Checkout OrderViewModel view
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Checkout()
        {
            bool IsUserLoggedin = User.Identity.IsAuthenticated;
            ShoppingCart? cart = null;
            List<Address> addresses = new List<Address>();
            List<Order> orders = new List<Order>();


            if (IsUserLoggedin)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string userId = user.Id;
                ViewBag.UserId = userId;
                addresses = await _context.Addresses
                    .Where(a => a != null && a.UserId == userId)
                    .ToListAsync();
                cart = await _shoppingCartService.GetShoppingCartAsync(userId);
            }

            var cartItemsInCart = await _context.ShoppingCartItems
                .Where(i => i.ShoppingCartId == cart.Id)
                .Include(c => c.Product)
                .ToListAsync();

            decimal totalPrice = _shoppingCartService.TotalPriceOfCart(cart);
            ViewBag.TotalPrice = totalPrice;
            var tables = new OrderViewModel
            {
                ShoppingCart = cart,
                CartItems = cartItemsInCart,
                Addresses = addresses,
                TotalPrice = totalPrice,

            };
            return await Task.Run(() => View(tables));
        }
        /// <summary>
        /// Post method for creating orders
        /// </summary>
        /// <param name="ovm"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Checkout(OrderViewModel ovm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Checkout");
            }
            if (ovm.AddressId == 0)
            {
                return await Task.Run(() => RedirectToAction("Checkout"));
            }
            DateTime date = DateTime.Now;

            ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;

            ShoppingCart cart = await _shoppingCartService.GetShoppingCartAsync(userId);

            decimal totalPrice = _shoppingCartService.TotalPriceOfCart(cart);
            if (totalPrice == 0)
            {
                return await Task.Run(() => RedirectToAction("Checkout"));
            }
            var cartItems = await _context.ShoppingCartItems
                .Where(i => i.ShoppingCartId == cart.Id)
                .ToListAsync();

            Order newOrder = new Order
            {
                UserId = userId,
                AddressId = ovm.AddressId,
                OrderDate = date,
                TotalPrice = totalPrice,
            };

            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            var newOrderItems = new List<OrderItem>();
            foreach (var item in cartItems)
            {
                OrderItem orderItem = new OrderItem
                {
                    OrderID = newOrder.Id,
                    ProductID = item.ProductId,
                    UserID = user.Id,
                    ProductName = item.Name,
                    Quantity = item.Quantity,
                    ImgUrl = item.ImgUrl,
                    UnitPrice = item.UnitPrice,
                };
                newOrderItems.Add(orderItem);
            }
            await _context.OrderItems.AddRangeAsync(newOrderItems);
            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            var successOrder = await _context.Orders.Where(o => o.Id == newOrder.Id).Include(o => o.OrderItems).FirstOrDefaultAsync();
            return await Task.Run(() => View("Success", newOrder));
        }

        /// <summary>
        /// Get Success OrderViewModel view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Success()
        {
            return View();
        }

        /// <summary>
        /// Increases cartItem's quantity by 1
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns>Checkout view</returns>
        public async Task<ActionResult> IncreaseItem(string cartItemId)
        {
            CartItem cartItem = await _context.ShoppingCartItems.FindAsync(cartItemId);
            cartItem.Quantity++;
            await _context.SaveChangesAsync();
            return await Task.Run(() => RedirectToAction(nameof(Checkout)));
        }

        /// <summary>
        /// Decreases cartItem's quantity by 1 or remove if new quantity is 0
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns>Checkout view</returns>
        public async Task<ActionResult> DecreaseItem(string cartItemId)
        {
            CartItem cartItem = await _context.ShoppingCartItems.FindAsync(cartItemId);
            cartItem.Quantity--;
            if (cartItem.Quantity <= 0)
            {
                _context.ShoppingCartItems.Remove(cartItem);
            }
            await _context.SaveChangesAsync();
            return await Task.Run(() => RedirectToAction(nameof(Checkout)));
        }

        /// <summary>
        /// Removes cartItem
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns>Checkout view</returns>
        public async Task<ActionResult> RemoveItem(string cartItemId)
        {
            CartItem cartItem = await _context.ShoppingCartItems.FindAsync(cartItemId);
            _context.ShoppingCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return await Task.Run(() => RedirectToAction(nameof(Checkout)));
        }
    }
}
