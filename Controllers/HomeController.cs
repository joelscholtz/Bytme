using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bytme.Models;
using System.Net.Mail;
using System.Net;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using bytme.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Linq.Expressions;
using System.Web;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Globalization;

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

        public async Task<IActionResult> Products(string searchString, string gender, string sortOrder, string categoryMen, string categoryWomen, string currentColor, string currentBrands)
        {
            var products = from p in _context.Items select p;
            ViewData["SortOrder"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["Female"] = String.IsNullOrEmpty(gender) ? "female" : "";
            ViewData["Male"] = String.IsNullOrEmpty(gender) ? "male" : "male";

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.price);
                    break;
                case "Date":
                    products = products.OrderBy(s => s.description);
                    break;
                case "date_desc":
                    products = products.OrderByDescending(s => s.description);
                    break;
                default:
                    products = products.OrderBy(s => s.price);
                    break;
            }

            switch(gender)
            {
                case "female":
                    products = products.Where(o => o.gender == "female");
                    break;
                case "male":
                    products = products.Where(o => o.gender == "male");
                    break;
            }

            ViewBag.Gender = gender;

            switch (categoryMen)
            {
                case "Blazers":
                    products = products.Where(o => o.long_description.Contains("Blazer") || o.long_description.Contains("blazer") && !o.long_description.Contains("navy blazer"));
                    break;
                case "Blouses":
                    products = products.Where(o => o.long_description.Contains("Blouse") || o.long_description.Contains("blouse"));
                    break;
                case "Jeans":
                    products = products.Where(o => o.long_description.Contains("Jeans") || o.long_description.Contains("jeans"));
                    break;
                case "Laptop Bags":
                    products = products.Where(o => o.long_description.Contains("Laptop bag") || o.long_description.Contains("laptop bag"));
                    break;
                case "Shirts":
                    products = products.Where(o => o.long_description.Contains("Shirt") || o.long_description.Contains("shirt"));
                    break;
                case "Suit Jackets":
                    products = products.Where(o => (o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket")));
                    break;
                case "Suit Pants":
                    products = products.Where(o => o.long_description.Contains("Suit pants") || o.long_description.Contains("suit pants"));
                    break;
                case "Suits":
                    products = products.Where(o => (o.long_description.Contains("Suit") || o.long_description.Contains("Suit")) && !(o.long_description.Contains("Suit Pants") || o.long_description.Contains("suit pants")) && !(o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket")));
                    break;
                case "Short Coats":
                    products = products.Where(o => o.long_description.Contains("Short coat") || o.long_description.Contains("short coat"));
                    break;
                case "Waist Coats":
                    products = products.Where(o => o.long_description.Contains("Waist coats") || o.long_description.Contains("waist coats"));
                    break;
            }

            switch (categoryWomen)
            {
                case "Blazers":
                    products = products.Where(o => o.long_description.Contains("Blazer") || o.long_description.Contains("blazer"));
                    break;
                case "Blouses":
                    products = products.Where(o => o.long_description.Contains("Blouse") || o.long_description.Contains("blouse"));
                    break;
                case "Boots":
                    products = products.Where(o => o.long_description.Contains("Boots") || o.long_description.Contains("boots"));
                    break;
                case "Coats":
                    products = products.Where(o => o.long_description.Contains("Coat") || o.long_description.Contains("coat"));
                    break;
                case "Cardigans":
                    products = products.Where(o => o.long_description.Contains("Cardigan") || o.long_description.Contains("cardigan"));
                    break;
                case "Dresses":
                    products = products.Where(o => (o.long_description.Contains("Dress") || o.long_description.Contains("dress")) && !o.long_description.Contains("shirt"));
                    break;
                case "Heels":
                    products = products.Where(o => o.long_description.Contains("Heels") || o.long_description.Contains("heels"));
                    break;
                case "Jumpers":
                    products = products.Where(o => o.long_description.Contains("Jumper") || o.long_description.Contains("jumper"));
                    break;
                case "Skirts":
                    products = products.Where(o => o.long_description.Contains("Skirt") || o.long_description.Contains("skirt"));
                    break;
                case "Trousers":
                    products = products.Where(o => o.long_description.Contains("Trousers") || o.long_description.Contains("trousers"));
                    break;
            }

            ViewBag.CurrentCategoryWomen = categoryWomen;
            ViewBag.CurrentCategoryMen = categoryMen;

            switch (currentColor)
            {
                case "red":
                    products = products.Where(o => o.long_description.Contains("red"));
                    break;
                case "blue":
                    products = products.Where(o => o.long_description.Contains("blue"));
                    break;
                case "pink":
                    products = products.Where(o => o.long_description.Contains("pink"));
                    break;
                case "green":
                    products = products.Where(o => o.long_description.Contains("green"));
                    break;
                case "yellow":
                    products = products.Where(o => o.long_description.Contains("yellow"));
                    break;
                case "orange":
                    products = products.Where(o => o.long_description.Contains("orange"));
                    break;
                case "cyan":
                    products = products.Where(o => o.long_description.Contains("cyan"));
                    break;
                case "black":
                    products = products.Where(o => o.long_description.Contains("black"));
                    break;
                case "white":
                    products = products.Where(o => o.long_description.Contains("white"));
                    break;
            }

            ViewBag.CurrentColor = currentColor;
            ViewData["CurrentBrands"] = currentBrands;
            ViewBag.CurrentBrands = products.Select(o => o.description).Distinct().ToList();

            ViewData["SearchString"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.description.ToUpper().Contains(searchString.ToUpper()) || s.long_description.ToLower().Contains(searchString.ToLower()));
                ViewBag.Message = searchString;
                ViewBag.Count = products.Where(s => s.description.ToUpper().Contains(searchString.ToUpper()) || s.long_description.ToLower().Contains(searchString.ToLower())).Count();
            }

            int PageCount = products.Count() / 21;
            ViewBag.PageCount = PageCount;
            ViewBag.BrandCount = products.Select(o => o.description).Distinct().Count();

            return View(await products.ToListAsync());
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

        public async Task<IActionResult> Men(string category, string maxPrice, string minPrice, string currentCategory, string currentBrands, IEnumerable<string> BrandBox, IEnumerable<string> ColorBox, string sortOrder)
        {
            // Incoming information from the form.
            ViewData["BrandBox"] = BrandBox;
            ViewData["maxPrice"] = maxPrice;
            ViewData["minPrice"] = minPrice;
            ViewData["ColorBox"] = ColorBox;
            ViewData["category"] = category;
            ViewData["currentCategory"] = currentCategory;
            ViewData["currentBrands"] = currentBrands;

            if (currentCategory != null)
            {
                ViewBag.currentCategory = currentCategory;
            }

            if (minPrice == null)
            {
                ViewBag.miniPrice = 12;
            }
            else
            {
                ViewBag.miniPrice = minPrice;
            }
            if (maxPrice == null)
            {
                ViewBag.maxiPrice = 695;
            }
            else
            {
                ViewBag.maxiPrice = maxPrice;
            }


            IList<string> Selected_BrandList = new List<string>();
            IList<string> Selected_ColorList = new List<string>();


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
            products = products.Where(o => o.gender == "male");

            ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();

            if (HttpContext.Request.Method == "POST")
            {


                //Array definitions for where clause and filtersColor
                Expression<Func<Item, bool>>[] WhereColor = new Expression<Func<Item, bool>>[300];

                IQueryable<Item>[] filtersColor = new IQueryable<Item>[300];

                //bools for if 1 checkbox has been checked
                bool ColorBox_OnlyOne = false;



                //COLORBOX MANAGEMENT {


                //for every colorbox selected create a where clause that filtersColor color and put the result in the where clause array

                int i = 0;
                foreach (var filter in ColorBox)
                {
                    WhereColor[i++] = a => a.long_description.Contains(filter);
                }
                if (i == 1)
                {
                    ColorBox_OnlyOne = true;
                }

                //for every wheereclause that is not null create an IQueryable with the WhereColor and put it in the array for filtersColor
                //every previous filter will be merged with the current filter
                bool inital = true;
                i = 0;
                IQueryable<Item> resultsColor = products;
                foreach (var clause in WhereColor)
                {

                    if (clause != null)
                    {
                        if (ColorBox_OnlyOne)
                        {
                            resultsColor = filtersColor[0] = products.Where(clause);
                        }
                        else if (ColorBox_OnlyOne == false)
                        {

                            filtersColor[i] = products.Where(clause);
                            if (i >= 1)
                            {
                                if (inital)
                                {
                                    resultsColor = Queryable.Union(
                                    filtersColor[i].AsQueryable(),
                                    filtersColor[i - 1].AsQueryable()
                                    );
                                    inital = false;
                                }
                                else if (inital == false)
                                {
                                    resultsColor = Queryable.Union(
                                    resultsColor, filtersColor[i]
                                    );
                                }

                            }

                            i++;
                        }
                    }


                }
                //}

                //BRANDBOX LIST {
                //Array definitions for where clause and filters
                Expression<Func<Item, bool>>[] whereBrand = new Expression<Func<Item, bool>>[300];

                IQueryable<Item>[] filtersBrand = new IQueryable<Item>[300];

                //for every colorbox selected create a where clause that filterBrand color and put the result in the where clause array
                bool BrandBox_OnlyOne = false;
                int j = 0;
                foreach (var filter in BrandBox)
                {
                    whereBrand[j++] = a => a.description == filter;
                }
                if (j == 1)
                {
                    BrandBox_OnlyOne = true;
                }
                //for every wheereclause that is not null create an IQueryable with the whereBrand and put it in the array for filterBrand
                //every previous filter will be merged with the current filter
                j = 0;
                bool initial = true;
                IQueryable<Item> resultsBrand = products;
                foreach (var clause in whereBrand)
                {
                    if (clause != null)
                    {
                        if (BrandBox_OnlyOne)
                        {
                            resultsBrand = filtersBrand[0] = products.Where(clause);
                        }
                        else if (BrandBox_OnlyOne == false)
                        {

                            filtersBrand[j] = products.Where(clause);
                            if (j >= 1 && initial == true)
                            {
                                resultsBrand = Queryable.Union(
                                filtersBrand[j].AsQueryable(),
                                filtersBrand[j - 1].AsQueryable()
                                );
                                initial = false;
                            }
                            else if (j >= 1 && initial == false)
                            {
                                resultsBrand = Queryable.Union(
                                    resultsBrand, filtersBrand[j]
                                    );
                            }


                            j++;
                        }
                    }
                }
                //}
                //final products list
                if (resultsColor != products && resultsBrand != products)
                {
                    IQueryable<Item> endresult = products;
                    for (int count = 0; count < ColorBox.Count(); count++)
                    {
                        string currCol = ColorBox.ToList()[count];
                        if (count == 0)
                        {

                            endresult = resultsBrand.Where(r => r.long_description.Contains(currCol));
                        }
                        else if (count >= 1)
                        {
                            IQueryable<Item> extraresult = resultsBrand.Where(r => r.long_description.Contains(currCol));
                            endresult = Queryable.Concat(endresult, extraresult);
                        }


                    }

                    products = endresult;

                }
                else if (resultsColor != products && resultsBrand == products)
                {

                    products = resultsColor;
                }
                else if (resultsBrand != products && resultsColor == products)
                {
                    products = resultsBrand;
                }

                //noncheckboxes (Fixed)
                if (ViewData["currentCategory"] != null)
                {
                    products = products.Where(p => p.long_description.Contains(ViewData["currentCategory"].ToString()) || p.long_description.Contains(ViewData["currentCategory"].ToString().ToLower()));
                    ViewBag.BrandSelected = products;
                    ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                    ViewBag.currentCategory = ViewData["currentCategory"];
                }
                if (minPrice != null && maxPrice != null)
                {
                    int minimum = Convert.ToInt32(minPrice);
                    int maximum = Convert.ToInt32(maxPrice);

                    if (maximum <= minimum)
                    {
                        int difference = Convert.ToInt32(minPrice) - Convert.ToInt32(maxPrice) + 1;
                        maximum = Convert.ToInt32(maxPrice) + difference;
                    }
                    else if (minimum >= maximum)
                    {
                        int difference = Convert.ToInt32(minPrice) - Convert.ToInt32(maxPrice) - 1;
                        minimum = Convert.ToInt32(minPrice) - difference;
                    }

                    ViewBag.maxiPrice = maximum;
                    ViewBag.miniPrice = minimum;

                    products = products.Where(p => p.price > minimum && p.price < maximum);
                    var blyat = products;
                }
                switch (category)
                {
                    case "All":
                        products = from p in _context.Items select p;
                        break;
                    case "Blazers":
                        products = products.Where(o => o.long_description.Contains("Blazer") || o.long_description.Contains("blazer") && !o.long_description.Contains("navy blazer"));
                        ViewBag.currentCategory = "Blazer";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Blouses":
                        products = products.Where(o => o.long_description.Contains("Blouse") || o.long_description.Contains("blouse"));
                        ViewBag.currentCategory = "Blouse";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Jeans":
                        products = products.Where(o => o.long_description.Contains("Jeans") || o.long_description.Contains("jeans"));
                        ViewBag.currentCategory = "Jeans";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Laptop Bags":
                        products = products.Where(o => o.long_description.Contains("Laptop bag") || o.long_description.Contains("laptop bag"));
                        ViewBag.currentCategory = "Laptop bag";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Shirts":
                        products = products.Where(o => o.long_description.Contains("Shirt") || o.long_description.Contains("shirt"));
                        ViewBag.currentCategory = "Shirt";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suit Jackets":
                        products = products.Where(o => (o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket")));
                        ViewBag.currentCategory = "Suit jacket";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suit Pants":
                        products = products.Where(o => o.long_description.Contains("Suit pants") || o.long_description.Contains("suit pants"));
                        ViewBag.currentCategory = "Suit pants";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suits":
                        products = products.Where(o => (o.long_description.Contains("Suit") || o.long_description.Contains("Suit")) && !(o.long_description.Contains("Suit Pants") || o.long_description.Contains("suit pants")) && !(o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket")));
                        ViewBag.currentCategory = "Suit";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Short Coats":
                        products = products.Where(o => o.long_description.Contains("Short coat") || o.long_description.Contains("short coat"));
                        ViewBag.currentCategory = "Short coat";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Waist Coats":
                        products = products.Where(o => o.long_description.Contains("Waist coats") || o.long_description.Contains("waist coats"));
                        ViewBag.currentCategory = "Waist coats";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                }
            }

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";



            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.price);
                    break;
                case "Date":
                    products = products.OrderBy(s => s.description);
                    break;
                case "date_desc":
                    products = products.OrderByDescending(s => s.description);
                    break;
                default:
                    products = products.OrderBy(s => s.price);
                    break;
            }
            int PageCount = products.Count() / 21;



            

            ViewBag.PageCount = PageCount;

            ViewBag.BrandCount = products.Select(o => o.description).Distinct().Count();


            return View(await products.ToListAsync());
        }
        
        public async Task<IActionResult> Women(string category, string maxPrice, string minPrice, string currentCategory, string currentBrands, IEnumerable<string> BrandBox, IEnumerable<string> ColorBox, string sortOrder)
        {
            // Incoming information from the form.
            ViewData["BrandBox"] = BrandBox;
            ViewData["maxPrice"] = maxPrice;
            ViewData["minPrice"] = minPrice;
            ViewData["ColorBox"] = ColorBox;
            ViewData["category"] = category;
            ViewData["currentCategory"] = currentCategory;
            ViewData["currentBrands"] = currentBrands;

            if (currentCategory != null)
            {
                ViewBag.currentCategory = currentCategory;
            }

            if (minPrice == null)
            {
                ViewBag.miniPrice = 12;
            }
            else
            {
                ViewBag.miniPrice = minPrice;
            }
            if (maxPrice == null)
            {
                ViewBag.maxiPrice = 695;
            }
            else
            {
                ViewBag.maxiPrice = maxPrice;
            }


            IList<string> Selected_BrandList = new List<string>();
            IList<string> Selected_ColorList = new List<string>();


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

            ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();

            if (HttpContext.Request.Method == "POST")
            {


                //Array definitions for where clause and filtersColor
                Expression<Func<Item, bool>>[] WhereColor = new Expression<Func<Item, bool>>[300];

                IQueryable<Item>[] filtersColor = new IQueryable<Item>[300];

                //bools for if 1 checkbox has been checked
                bool ColorBox_OnlyOne = false;



                //COLORBOX MANAGEMENT {


                //for every colorbox selected create a where clause that filtersColor color and put the result in the where clause array

                int i = 0;
                foreach (var filter in ColorBox)
                {
                    WhereColor[i++] = a => a.long_description.Contains(filter);
                }
                if (i == 1)
                {
                    ColorBox_OnlyOne = true;
                }

                //for every wheereclause that is not null create an IQueryable with the WhereColor and put it in the array for filtersColor
                //every previous filter will be merged with the current filter
                bool inital = true;
                i = 0;
                IQueryable<Item> resultsColor = products;
                foreach (var clause in WhereColor)
                {

                    if (clause != null)
                    {
                        if (ColorBox_OnlyOne)
                        {
                            resultsColor = filtersColor[0] = products.Where(clause);
                        }
                        else if (ColorBox_OnlyOne == false)
                        {

                            filtersColor[i] = products.Where(clause);
                            if (i >= 1)
                            {
                                if (inital)
                                {
                                    resultsColor = Queryable.Union(
                                    filtersColor[i].AsQueryable(),
                                    filtersColor[i - 1].AsQueryable()
                                    );
                                    inital = false;
                                }
                                else if (inital == false)
                                {
                                    resultsColor = Queryable.Union(
                                    resultsColor, filtersColor[i]
                                    );
                                }

                            }

                            i++;
                        }
                    }


                }
                //}

                //BRANDBOX LIST {
                //Array definitions for where clause and filters
                Expression<Func<Item, bool>>[] whereBrand = new Expression<Func<Item, bool>>[300];

                IQueryable<Item>[] filtersBrand = new IQueryable<Item>[300];

                //for every colorbox selected create a where clause that filterBrand color and put the result in the where clause array
                bool BrandBox_OnlyOne = false;
                int j = 0;
                foreach (var filter in BrandBox)
                {
                    whereBrand[j++] = a => a.description == filter;
                }
                if (j == 1)
                {
                    BrandBox_OnlyOne = true;
                }
                //for every wheereclause that is not null create an IQueryable with the whereBrand and put it in the array for filterBrand
                //every previous filter will be merged with the current filter
                j = 0;
                bool initial = true;
                IQueryable<Item> resultsBrand = products;
                foreach (var clause in whereBrand)
                {
                    if (clause != null)
                    {
                        if (BrandBox_OnlyOne)
                        {
                            resultsBrand = filtersBrand[0] = products.Where(clause);
                        }
                        else if (BrandBox_OnlyOne == false)
                        {

                            filtersBrand[j] = products.Where(clause);
                            if (j >= 1 && initial == true)
                            {
                                resultsBrand = Queryable.Union(
                                filtersBrand[j].AsQueryable(),
                                filtersBrand[j - 1].AsQueryable()
                                );
                                initial = false;
                            }
                            else if (j >= 1 && initial == false)
                            {
                                resultsBrand = Queryable.Union(
                                    resultsBrand, filtersBrand[j]
                                    );
                            }


                            j++;
                        }
                    }
                }
                //}
                //final products list
                if (resultsColor != products && resultsBrand != products)
                {
                    IQueryable<Item> endresult = products;
                    for (int count = 0; count < ColorBox.Count(); count++)
                    {
                        string currCol = ColorBox.ToList()[count];
                        if (count == 0)
                        {

                            endresult = resultsBrand.Where(r => r.long_description.Contains(currCol));
                        }
                        else if (count >= 1)
                        {
                            IQueryable<Item> extraresult = resultsBrand.Where(r => r.long_description.Contains(currCol));
                            endresult = Queryable.Concat(endresult, extraresult);
                        }


                    }

                    products = endresult;

                }
                else if (resultsColor != products && resultsBrand == products)
                {

                    products = resultsColor;
                }
                else if (resultsBrand != products && resultsColor == products)
                {
                    products = resultsBrand;
                }

                //noncheckboxes (Fixed)
                if (ViewData["currentCategory"] != null)
                {
                    products = products.Where(p => p.long_description.Contains(ViewData["currentCategory"].ToString()) || p.long_description.Contains(ViewData["currentCategory"].ToString().ToLower()));
                    ViewBag.BrandSelected = products;
                    ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                    ViewBag.currentCategory = ViewData["currentCategory"];
                }
                if (minPrice != null && maxPrice != null)
                {
                    int minimum = Convert.ToInt32(minPrice);
                    int maximum = Convert.ToInt32(maxPrice);

                    if (maximum <= minimum)
                    {
                        int difference = Convert.ToInt32(minPrice) - Convert.ToInt32(maxPrice) + 1;
                        maximum = Convert.ToInt32(maxPrice) + difference;
                    }
                    else if (minimum >= maximum)
                    {
                        int difference = Convert.ToInt32(minPrice) - Convert.ToInt32(maxPrice) - 1;
                        minimum = Convert.ToInt32(minPrice) - difference;
                    }

                    ViewBag.maxiPrice = maximum;
                    ViewBag.miniPrice = minimum;

                    products = products.Where(p => p.price > minimum && p.price < maximum);
                    var blyat = products;
                }
                switch (category)
                {
                    case "All":
                        products = from p in _context.Items select p;
                        break;
                    case "Blazers":
                        products = products.Where(o => o.long_description.Contains("Blazer") || o.long_description.Contains("blazer"));
                        ViewBag.currentCategory = "Blazer";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Blouses":
                        products = products.Where(o => o.long_description.Contains("Blouse") || o.long_description.Contains("blouse"));
                        ViewBag.currentCategory = "Blouse";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Boots":
                        products = products.Where(o => o.long_description.Contains("Boots") || o.long_description.Contains("boots"));
                        ViewBag.currentCategory = "Boots";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Coats":
                        products = products.Where(o => o.long_description.Contains("Coat") || o.long_description.Contains("coat"));
                        ViewBag.currentCategory = "Coat";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Cardigans":
                        products = products.Where(o => o.long_description.Contains("Cardigan") || o.long_description.Contains("cardigan"));
                        ViewBag.currentCategory = "Cardigan";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Dresses":
                        products = products.Where(o => (o.long_description.Contains("Dress") || o.long_description.Contains("dress")) && !o.long_description.Contains("shirt"));
                        ViewBag.currentCategory = "Dress";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Heels":
                        products = products.Where(o => o.long_description.Contains("Heels") || o.long_description.Contains("heels"));
                        ViewBag.currentCategory = "Heels";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Jumpers":
                        products = products.Where(o => o.long_description.Contains("Jumper") || o.long_description.Contains("jumper"));
                        ViewBag.currentCategory = "Jumper";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Skirts":
                        products = products.Where(o => o.long_description.Contains("Skirt") || o.long_description.Contains("skirt"));
                        ViewBag.currentCategory = "Skirt";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Trousers":
                        products = products.Where(o => o.long_description.Contains("Trousers") || o.long_description.Contains("trousers"));
                        ViewBag.currentCategory = "Trousers";
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                }
            }

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";



            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.price);
                    break;
                case "Date":
                    products = products.OrderBy(s => s.description);
                    break;
                case "date_desc":
                    products = products.OrderByDescending(s => s.description);
                    break;
                default:
                    products = products.OrderBy(s => s.price);
                    break;
            }
            int PageCount = 1;
            double doublePageCount = products.Count() / 21;
            if (doublePageCount - Convert.ToInt16(doublePageCount) != 0) { PageCount = Convert.ToInt16(doublePageCount) + 1; }



            List<int> Pages = new List<int> { };
            for (int i = 0; i < PageCount; i++)
            {
                Pages.Add(i);
            }

            ViewBag.PageList = Pages;

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
