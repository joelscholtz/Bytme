using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using bytme.Models;
using bytme.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace bytme.Components
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;

        public CartViewComponent(
            ApplicationDbContext context, 
            SignInManager<UserModel> signInManager, 
            UserManager<UserModel> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public int CheckIfOrderExists()
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            OrderMain orderMain = _context.OrderMains.Where(o => o.user_id == userId).FirstOrDefault();
            if (orderMain != null)
            {
                return orderMain.id;
            }
            else
            {
                return 0;
            }

        }

        public int CountItemsInShoppingCart(int order_id)
        {
            var result = (from orderlines in _context.OrderLines
                          join items in _context.Items on orderlines.item_id equals items.id
                          join ordermains in _context.OrderMains on orderlines.order_id equals ordermains.id
                          where orderlines.order_id == order_id
                          select orderlines).Count();

            return result;
        }

        public IViewComponentResult Invoke()
        {
            int order_id = CheckIfOrderExists();
            int count = CountItemsInShoppingCart(order_id);
            ViewBag.count = count;

            return View(ViewBag.count);
        }
    }
}
