using bytme.Data;
using bytme.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace bytme.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult ProductDetail(int id)
        {
            Item item = _context.Items.Where(o => o.id == id).FirstOrDefault();

            var model = new Item();
            model.id = item.id;
            model.description = item.description;
            model.long_description = item.long_description;
            model.photo_url = item.photo_url;
            model.price = item.price;
            model.quantity = item.quantity;
            model.size = item.size;
            model.issales = item.issales;
            model.gender = item.gender;

            return View(model);
        }
    }
}
