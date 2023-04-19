using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Helper;
using ShoppingApp.Models;
using ShoppingApp.Services;
using ShoppingApp.ViewModels;

namespace ShoppingApp.Controllers
{
    public class OrderArchiveViewModelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartService _shoppingCartService;
        private UserManager<ApplicationUser> _userManager;

        public OrderArchiveViewModelController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
            , IShoppingCartService shoppingCartService)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = (ShoppingCartService)shoppingCartService;
        }



        // GET: OrderArchiveController
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Index(int? page)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;

            var OrderList = _context.Orders.Where(o => o != null && o.UserId == userId).OrderByDescending(o => o.OrderDate).Include(o => o.OrderItems);

            int pageSize = 5;
            int pageNumber = page ?? 1;

            var pagedOrderList = new PagedList<Order>(OrderList.ToList(), OrderList.Count(), pageNumber, pageSize);
            var displayedOrderList = PagedList<Order>.ToPagedList(OrderList, pageNumber, pageSize);

            /*var OrderItemList = new List<List<OrderItem>> { };
            foreach (var order in OrderList) 
            { 
                order.OrderItems
            }*/


            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            ViewBag.TotalPages = pagedOrderList.TotalPages;
            ViewBag.HasNext = pagedOrderList.HasNext;
            ViewBag.HasPrevious = pagedOrderList.HasPrevious;

            var lists = new OrderArchiveViewModel
            {
                Orders = displayedOrderList,
                // OrderItems =
            };

            return await Task.Run(() => View(lists));

        }

        // GET: OrderArchiveController/Details/5
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Details(int orderId)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;

            Order order = _context.Orders
                .Where(o => o != null && o.Id == orderId)
                .Include(o => o.OrderItems)
                .Include(o => o.Address).FirstOrDefault();
            if (order.UserId == userId)
            {
                return await Task.Run(() => View(order));
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
