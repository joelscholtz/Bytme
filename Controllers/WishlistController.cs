using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using bytme.Data;
using bytme.Models;
using System.Net;

namespace bytme.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;


        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            IQueryable<WishlistModel> model = null;
            int wishlist_id = Check();

            if (wishlist_id != 0)
            {
                var WishLines = _context.WishLines.Where(l => l.Wishmain_id == wishlist_id).ToList();
                if (WishLines.Count() > 0)
                {
                    model = from wishlines in _context.WishLines
                            join items in _context.Items on wishlines.itm_id equals items.id
                            join wishmain in _context.WishMains on wishlines.Wishmain_id equals wishmain.id
                            where wishlines.Wishmain_id == wishlist_id
                            select new WishlistModel
                            {
                                long_description = items.long_description,
                                stock = items.quantity,
                                description = items.description,
                                price = items.price,
                                ordline_id = wishlines.id,
                                photo_url = items.photo_url,
                                item_id = items.id
                            };
                    ViewBag.model_for_view = model;
                    return View(model.ToList());
                }
            }
            ViewBag.model_for_view = model;
            return View();
        }

        public int Check()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

        public int TotalCount()
        {
            int id = Check();
            int count = CountWishList(id);
            ViewBag.countWish = count;

            return ViewBag.countWish;
        }

        public async Task<ActionResult> AddToWishList(int itm_id, int recordId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishMain = _context.WishMains.Where(w => w.user_ad == userId).FirstOrDefault();

            if (wishMain != null)
            {
                recordId = wishMain.id;
            }
            else
            {
                WishMains wish = new WishMains();
                wish.user_ad = userId;
                wish.dt_created = DateTime.Now;
                _context.Add(wish);
                await _context.SaveChangesAsync();

                recordId = wish.id;
            }

            var check = (from m in _context.WishLines
                         where m.itm_id == itm_id && m.Wishmain_id == recordId
                         select m).FirstOrDefault();

            if (check == null)
            {
                WishLines wishline = new WishLines();
                wishline.itm_id = itm_id;
                wishline.Wishmain_id = recordId;
                wishline.dt_created = DateTime.Now;

                _context.Add(wishline);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> RemoveFromWishList(int ordline_id)
        {
            var result = (from wishlines in _context.WishLines
                          where wishlines.id == ordline_id
                          select wishlines).FirstOrDefault();
            if (result != null)
            {
                _context.WishLines.Remove(result);
                await _context.SaveChangesAsync();
            }
            else
            {
                ViewBag.ErrorMessage = "Product cannot be removed!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
