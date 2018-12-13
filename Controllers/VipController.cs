using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bytme.Models;
using bytme.Data;
using System.Security.Claims;

namespace bytme.Controllers
{
    public class VipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VipController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Join()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _context.UserModels.Where(u => u.Id == userId).FirstOrDefault();
            result.points = 50;
            _context.Update(result);
            _context.SaveChanges();

            return RedirectToAction("Vip");
        }

        public ActionResult Vip()
        {
            return View();
        }
    }
}
