using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace bytme.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Delivery()
        {
            return View();
        }

        public IActionResult Payments()
        {
            return View();
        }

        public IActionResult Return()
        {
            return View();
        }

        public IActionResult Sizing()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }
    }
}