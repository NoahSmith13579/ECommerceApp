﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Models;
using ShoppingApp.Services;

namespace ShoppingApp.Controllers
{
    public class AddressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartService _shoppingCartService;
        private UserManager<ApplicationUser> _userManager;

        public AddressController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
            , IShoppingCartService shoppingCartService)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = (ShoppingCartService)shoppingCartService;
        }
        /// <summary>
        /// Gets Address Create view
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Create(string userId)
        {
            ViewBag.UserId = userId;
            return await Task.Run(() => View());
        }

        /// <summary>
        /// Post method for creating addresses
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Bind, City, Street, Street2, Country, ZipCode, UserId")] Address address)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id;
            address.UserId = userId;
            if (ModelState.IsValid)
            {
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
            }
            else
            {
                return View();
            }
            return RedirectToAction("Checkout", "OrderViewModel");
        }

        /// <summary>
        /// Gets Address Edit view
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Edit(int addressId)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
            ViewBag.UserId = user.Id;

            Address address = await _context.Addresses.FirstAsync(a => a.Id == addressId);

            return View(address);
        }
        /// <summary>
        /// Post method for editting address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id, Bind, City, Street, Street2, Country, ZipCode, UserId")] Address address)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(address).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
            {
                return View();
            }
            return RedirectToAction("Checkout", "OrderViewModel");
        }
    }
}
