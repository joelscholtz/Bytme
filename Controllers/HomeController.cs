using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bytme.Models;
using System.Net.Mail;
using System.Net;
using bytme.Data;
using Microsoft.EntityFrameworkCore;

namespace bytme.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            IEnumerable<Item> item = _context.Items.AsEnumerable();

            return View(item);
        }

        public ActionResult Products()
        {
            IEnumerable<Item> item = _context.Items.AsEnumerable();

            return View(item);
        }

        public IActionResult About()
        {
            //ViewData["Message"] = "Hello @UserManager.GetUserName(User)";

            return View();
        }

        public IActionResult Contact()
        {
            //ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Men(
    string sortOrder,
    int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";


            var items = from s in _context.Items
                        select s;
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.price);
                    break;
                case "Date":
                    items = items.OrderBy(s => s.description);
                    break;
                case "date_desc":
                    items = items.OrderByDescending(s => s.description);
                    break;
                default:
                    items = items.OrderBy(s => s.price);
                    break;
            }
            int pageSize = 21;
            return View(await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), page ?? 1, pageSize));
        }

        public async Task<IActionResult> Women(string category, string maxPrice, string minPrice, string currentCat, string currentBrand, string BrandBox, string ColorBox)
        {
            // Incoming information from the form.
            ViewData["BrandBox"] = BrandBox;
            ViewData["maxPrice"] = maxPrice;
            ViewData["minPrice"] = minPrice;
            ViewData["ColorBox"] = ColorBox;
            ViewData["category"] = category;
            ViewData["currentCat"] = currentCat;
            ViewData["currentBrand"] = currentBrand;

            ViewBag.miniPrice = minPrice;
            ViewBag.maxiPrice = maxPrice;

            IList<string> clrList = new List<string>();
            clrList.Add("red");
            clrList.Add("blue");
            clrList.Add("pink");
            clrList.Add("green");
            clrList.Add("yellow");
            clrList.Add("orange");
            clrList.Add("cyan");
            clrList.Add("black");
            clrList.Add("white");
            //clrList.Add("grey");
            //clrList.Add("cream");
            //clrList.Add("navy");
            //clrList.Add("nero");
            //clrList.Add("caviar");
            //clrList.Add("sapphire");
            //clrList.Add("charcoal");
            //clrList.Add("nutmeg");
            //clrList.Add("brown");
            //clrList.Add("burgundy");
            //clrList.Add("marine");
            //clrList.Add("sky captain");

            ViewBag.ColorList = clrList;
            // Retrieve the products from the database.
            var products = from p in _context.Items select p;
            products = products.Where(o => o.gender == "female");

            if (HttpContext.Request.Method == "POST")
            {
                switch (category)
                {
                    case "All":
                        products = from p in _context.Items select p;
                        break;
                    case "Blazers":
                        products = products.Where(o => o.long_description.Contains("Blazer") || o.long_description.Contains("blazer"));
                        ViewBag.currentCategory = "Blazer";
                        break;
                    case "Blouses":
                        products = products.Where(o => o.long_description.Contains("Blouse") || o.long_description.Contains("blouse"));
                        ViewBag.currentCategory = "Blouse";
                        break;
                    case "Boots":
                        products = products.Where(o => o.long_description.Contains("Boots") || o.long_description.Contains("boots"));
                        ViewBag.currentCategory = "Boots";
                        break;
                    case "Coats":
                        products = products.Where(o => o.long_description.Contains("Coat") || o.long_description.Contains("coat"));
                        ViewBag.currentCategory = "Coat";
                        break;
                    case "Cardigans":
                        products = products.Where(o => o.long_description.Contains("Cardigan") || o.long_description.Contains("cardigan"));
                        ViewBag.currentCategory = "Cardigan";
                        break;
                    case "Dresses":
                        products = products.Where(o => (o.long_description.Contains("Dress") || o.long_description.Contains("dress")) && !o.long_description.Contains("shirt"));
                        ViewBag.currentCategory = "Dress";
                        break;
                    case "Heels":
                        products = products.Where(o => o.long_description.Contains("Heels") || o.long_description.Contains("heels"));
                        ViewBag.currentCategory = "Heels";
                        break;
                    case "Jumpers":
                        products = products.Where(o => o.long_description.Contains("Jumper") || o.long_description.Contains("jumper"));
                        ViewBag.currentCategory = "Jumper";
                        break;
                    case "Skirts":
                        products = products.Where(o => o.long_description.Contains("Skirt") || o.long_description.Contains("skirt"));
                        ViewBag.currentCategory = "Skirt";
                        break;
                    case "Trousers":
                        products = products.Where(o => o.long_description.Contains("Trousers") || o.long_description.Contains("trousers"));
                        ViewBag.currentCategory = "Trousers";
                        break;


                }
                if (ViewData["maxPrice"] != null && float.Parse(ViewData["maxPrice"].ToString()) != 0f)
                {

                    if (ViewData["currentCat"] != null)
                    {
                        products = products.Where(p => p.price <= float.Parse(ViewData["maxPrice"].ToString()) && (p.long_description.Contains(currentCat) || p.long_description.Contains(currentCat.ToLower())));
                        ViewBag.currentCategory = currentCat;
                    }
                    else
                    {
                        products = products.Where(p => p.price <= float.Parse(ViewData["maxPrice"].ToString()));
                    }

                }
            }

            if (ViewData["minPrice"] != null && float.Parse(ViewData["minPrice"].ToString()) != 0f)
            {
                if (ViewData["currentCat"] != null)
                {
                    products = products.Where(p => p.price >= float.Parse(ViewData["minPrice"].ToString()) && (p.long_description.Contains(currentCat) || p.long_description.Contains(currentCat.ToLower())));
                    ViewBag.currentCategory = currentCat;
                }
                else
                {
                    products = products.Where(p => p.price >= float.Parse(ViewData["minPrice"].ToString()));
                }

            }
            if (ViewData["BrandBox"] != null)
            {


                if (currentCat != null)
                {

                    products = products.Where(p => p.description == ViewData["BrandBox"].ToString() && (p.long_description.Contains(currentCat) || p.long_description.Contains(currentCat.ToLower())));
                    ViewBag.currentCategory = currentCat;
                    ViewBag.PreviousBrand = products.Select(o => o.description).Distinct();
                    ViewBag.currentBrand = ViewData["BrandBox"].ToString();
                }
                else
                {
                    products = products.Where(p => p.description == ViewData["BrandBox"].ToString());
                    ViewBag.PreviousBrand = products.Select(o => o.description).Distinct();
                    ViewBag.currentBrand = ViewData["BrandBox"].ToString();
                }

            }
            if (ViewData["ColorBox"] != null)
            {

                if (currentBrand != null && currentCat != null)
                {
                    products = products.Where(p => p.long_description == ViewData["BrandBox"].ToString() && p.long_description.Contains(ViewData["ColorBox"].ToString()) && (p.long_description.Contains(currentCat) || p.long_description.Contains(currentCat.ToLower())));
                    ViewBag.currentBrand = currentBrand;
                }
                else if (currentBrand != null && currentCat == null)
                {
                    products = products.Where(p => p.long_description == ViewData["BrandBox"].ToString() && p.long_description.Contains(ViewData["ColorBox"].ToString()));
                    ViewBag.currentBrand = currentBrand;
                }
                else if (currentCat != null)
                {
                    products = products.Where(p => p.long_description.Contains(ViewData["ColorBox"].ToString()) && (p.long_description.Contains(currentCat) || p.long_description.Contains(currentCat.ToLower())));
                    ViewBag.currentCategory = currentCat;
                }

                else
                {
                    products = products.Where(p => p.long_description.Contains(ViewData["ColorBox"].ToString()));
                }

            }




            ViewBag.BrandCount = products.Select(o => o.description).Distinct().Count();

            return View(await products.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Contact(ContactModel c)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage sentMail = new MailMessage();
                    sentMail.IsBodyHtml = true;
                    // TODO Fix HTML weergave
                    sentMail.Body = " <div style='width:95%;'> " +
                                    "<br><h2> Our customer support will contact you as soon as possible</h2><br>" +
                                    " <table style='font-family: arial,sans-serif;border-collapse: collapse;width:75%; '>" +
                                    " <tr> " +
                                    " <th style='border: 1px solid #dddddd;text-align: left; padding:8px;font-size: 14pt;'>Name:</th>" +
                                    " <th style='border: 1px solid #dddddd;text-align: left; padding:8px;font-size: 14pt;'>Email:</th>" +
                                    " <th style='border: 1px solid #dddddd;text-align: left; padding:8px;font-size: 14pt;'>Subject:</th>" +
                                    " <th style='border: 1px solid #dddddd;text-align: left; padding:8px;font-size: 14pt;'>Message:</th>" +
                                    " </tr>" +
                                    " <tr>" +
                                    " <td style='border: 1px solid #dddddd;text-align: left; padding:8px;'>" + c.name + "</td>" +
                                    " <td style='border: 1px solid #dddddd;text-align: left; padding:8px;'>" + c.email + "</td>" +
                                    " <td style='border: 1px solid #dddddd;text-align: left; padding:8px;'>" + c.subject + "</td>" +
                                    " <td style='border: 1px solid #dddddd;text-align: left; padding:8px;'>" + c.content + "</td>" +
                                    "</table><br>" +
                                    "It's also possible to contact us via: 010 - 1234567 from monday till friday between 9:00 and 17:00." +
                                    "</div>";

                    sentMail.From = new MailAddress("sayoswebshop@gmail.com");
                    sentMail.To.Add(c.email);
                    sentMail.To.Add("sayoswebshop@gmail.com");
                    sentMail.Subject = c.subject;

                    SmtpClient client = new SmtpClient();
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("sayoswebshop@gmail.com", "PonyParkSlagHaren1234");


                    client.Send(sentMail);

                    ModelState.Clear();
                    ViewBag.Message = "Thanks for the message, we will contact you as soon as possible! ";
                }
                catch (Exception err)
                {
                    ModelState.Clear();
                    ViewBag.Message = $"There was a error: {err.Message}";
                }
            }

            return View();
        }

    }
}
