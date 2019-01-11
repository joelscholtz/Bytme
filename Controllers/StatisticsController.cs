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
            ViewBag.UsersJAN = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "januari").Count();
            ViewBag.UsersFEB = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "februari").Count();
            ViewBag.UsersMAR = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "maart").Count();
            ViewBag.UsersAPR = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "april").Count();
            ViewBag.UsersMAY = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "mei").Count();
            ViewBag.UsersJUN = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "juni").Count();
            ViewBag.UsersJUL = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "juli").Count();
            ViewBag.UsersAUG = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "augustus").Count();
            ViewBag.UsersSEP = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "september").Count();
            ViewBag.UsersOCT = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "oktober").Count();
            ViewBag.UsersNOV = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "november").Count();
            ViewBag.UsersDEC = _context.Users.Where(o => o.dt_created.ToString("MMMM") == "december").Count();

            if (ViewBag.UsersJAN == 0) { ViewBag.UsersJAN = null; }
            if (ViewBag.UsersFEB == 0) { ViewBag.UsersFEB = null; }
            if (ViewBag.UsersMAR == 0) { ViewBag.UsersMAR = null; }
            if (ViewBag.UsersAPR == 0) { ViewBag.UsersAPR = null; }
            if (ViewBag.UsersMAY == 0) { ViewBag.UsersMAY = null; }
            if (ViewBag.UsersJUN == 0) { ViewBag.UsersJUN = null; }
            if (ViewBag.UsersJUL == 0) { ViewBag.UsersJUL = null; }
            if (ViewBag.UsersAUG == 0) { ViewBag.UsersAUG = null; }
            if (ViewBag.UsersSEP == 0) { ViewBag.UsersSEP = null; }
            if (ViewBag.UsersOCT == 0) { ViewBag.UsersOCT = null; }
            if (ViewBag.UsersNOV == 0) { ViewBag.UsersNOV = null; }
            if (ViewBag.UsersDEC == 0) { ViewBag.UsersDEC = null; }

            //Moneymoneymomey
            ViewBag.MoneyJAN = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "januari").Sum(o => o.price_payed);
            ViewBag.MoneyFEB = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "februari").Sum(o => o.price_payed);
            ViewBag.MoneyMAR = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "maart").Sum(o => o.price_payed);
            ViewBag.MoneyAPR = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "april").Sum(o => o.price_payed);
            ViewBag.MoneyMAY = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "mei").Sum(o => o.price_payed);
            ViewBag.MoneyJUN = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "juni").Sum(o => o.price_payed);
            ViewBag.MoneyJUL = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "juli").Sum(o => o.price_payed);
            ViewBag.MoneyAUG = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "augustus").Sum(o => o.price_payed);
            ViewBag.MoneySEP = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "september").Sum(o => o.price_payed);
            ViewBag.MoneyOCT = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "oktober").Sum(o => o.price_payed);
            ViewBag.MoneyNOV = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "november").Sum(o => o.price_payed);
            ViewBag.MoneyDEC = _context.OrderHistories.Where(o => o.dt_created.ToString("MMMM") == "december").Sum(o => o.price_payed);

            return View();
        }
    }
}
