using bytme.Data;
using bytme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace bytme.Controllers
{
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<UserModel> _userManager;

        public ManageController(ApplicationDbContext db, UserManager<UserModel> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // user overview
        [Authorize(Roles = "Admin")]
        public ViewResult Index(string searchKeyU)
        {

            var users = from u in _db.Users
                        select u;
            if (!String.IsNullOrEmpty(searchKeyU))
            {
                //searches for items containing input searchkey
                // .ToUpper() is used to make search non-case sensitive
                users = _userManager.Users.Where(u => u.Id.ToUpper().Contains(searchKeyU.ToUpper())
                                       || u.name.ToUpper().Contains(searchKeyU.ToUpper())
                                       || u.surname.ToUpper().Contains(searchKeyU.ToUpper())
                                       || u.Email.ToUpper().Contains(searchKeyU.ToUpper()));
            }
            return View(users.ToList());
        }


        // product overview
        [Authorize(Roles = "Admin")]
        public ViewResult Products(string searchKeyP)
        {
            var products = from item in _db.Items
                           select item;
            if (!String.IsNullOrEmpty(searchKeyP))
            {

                //searches for items containing input searchkey
                products = _db.Items.Where(item => item.id.ToString().ToUpper().Contains(searchKeyP.ToUpper())
                                        || item.description.ToUpper().Contains(searchKeyP.ToUpper())
                                        || item.price.ToString().ToUpper().Contains(searchKeyP.ToUpper())
                                        || item.quantity.ToString().ToUpper().Contains(searchKeyP.ToUpper())
                                        || item.gender.ToUpper().Contains(searchKeyP.ToUpper()));
            }

            return View(products.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Statistics()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UserEdit()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult UserDelete()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ProductEdit()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ProductDelete()
        {
            return View();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }



            return View();
        }

    }
}
