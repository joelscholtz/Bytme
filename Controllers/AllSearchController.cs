using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bytme.Connector;
using Microsoft.AspNetCore.Mvc;

namespace bytme.Controllers
{
    public class AllSearchController : Controller
    {
        private readonly ConnectionToEs _connectionToEs;
        public AllSearchController()
        {
            _connectionToEs = new ConnectionToEs();
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View("Search");
        }
    }
}