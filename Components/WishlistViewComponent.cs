using bytme.Data;
using bytme.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace bytme.Components
{
    public class WishlistViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public WishlistViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public int Check()
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            WishMains wishMain = _context.WishMains.Where(o => o.user_ad == userId).FirstOrDefault();
            if (wishMain != null)
            {
                return wishMain.id;
            }
            else
            {
                return 0;
            }

        }

        public int CountWishList(int id)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = (from wishLines in _context.WishLines
                          join items in _context.Items on wishLines.itm_id equals items.id
                          join wishMains in _context.WishMains on wishLines.Wishmain_id equals wishMains.id
                          where wishLines.Wishmain_id == id
                          select wishLines).Count();

            return result;
        }

        public IViewComponentResult Invoke()
        {
            int id = Check();
            int count = CountWishList(id);
            ViewBag.countWish = count;

            return View(ViewBag.countWish);
        }
    }
}
