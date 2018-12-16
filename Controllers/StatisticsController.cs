using bytme.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bytme.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            //Users
            var months = _context.Users.ToList().Select(o => o.dt_created.ToString("MMMM")).ToList();
            var users = _context.Users.Count();
            ViewBag.Users = users;
            ViewBag.Months = string.Join(", ", months);

            //Moneymoneymomey
            var moneymonths = _context.OrderHistories.ToList().Select(o => o.dt_created.ToString("MMMM")).ToList();
            //var moneys = _context.OrderHistories.Select(o => o.price_payed.ToString()).ToList();
            float kutgeld = _context.OrderHistories.Sum(o => o.price_payed);

            ViewBag.Money = string.Join(", ", kutgeld);
            ViewBag.MoneyMonths = string.Join(", ", moneymonths);

            return View();
        }
    }
}
