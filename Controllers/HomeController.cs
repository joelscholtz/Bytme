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

        public async Task<IActionResult> Products(string searchString, string gender, string sortOrder, string categoryMen, string categoryWomen, string currentColor, string currentBrands, IEnumerable<string> BrandBox, string maxPrice, string minPrice, IEnumerable<string> ColorBox, string category, string currentCategory, string sortBy, string currentSort)
        {
            var products = from p in _context.Items select p;
            ViewData["SortOrder"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["Female"] = String.IsNullOrEmpty(gender) ? "female" : "";
            ViewData["Male"] = String.IsNullOrEmpty(gender) ? "male" : "male";

            ViewData["Gender"] = gender;
            ViewData["BrandBox"] = BrandBox;
            ViewData["maxPrice"] = maxPrice;
            ViewData["minPrice"] = minPrice;
            ViewData["ColorBox"] = ColorBox;
            ViewData["category"] = category;
            ViewData["currentCategory"] = currentCategory;
            ViewData["currentBrands"] = currentBrands;
            ViewData["sortBy"] = sortBy;
            ViewData["currentSort"] = currentSort;

            IList<string> clrList = new List<string>();
            clrList.Add("red");
            clrList.Add("blue");
            clrList.Add("green");
            clrList.Add("yellow");
            clrList.Add("black");
            clrList.Add("white");
            clrList.Add("navy");
            clrList.Add("grey");
            clrList.Add("brown");

            ViewBag.currentSort = "Recommended";
            bool SortByRemember = String.IsNullOrEmpty(sortBy);
            if (String.IsNullOrEmpty(sortBy)) { sortBy = "Recommended"; }
            ViewBag.CurrentCategoryWomen = categoryWomen;
            ViewBag.CurrentCategoryMen = categoryMen;
            ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
            ViewBag.maxiPrice = "1500";
            ViewBag.miniPrice = "0";
            if (HttpContext.Request.Method == "POST")
            {
                //Array definitions for where clause and filtersColor
                Expression<Func<Item, bool>>[] WhereColor = new Expression<Func<Item, bool>>[300];

                IQueryable<Item>[] filtersColor = new IQueryable<Item>[300];

                //bools for if 1 checkbox has been checked
                bool ColorBox_OnlyOne = false;

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
                
                if (ViewData["currentCategory"] != null)
                {
                    ViewBag.currentCategory = ViewData["currentCategory"];
                    if (String.IsNullOrEmpty(category)) { category = currentCategory; }
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
                switch (gender)
                {
                    case "Women":
                        products = products.Where(o => o.gender == "female");
                        ViewBag.Gender = gender;
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Men":
                        products = products.Where(o => o.gender == "male");
                        ViewBag.Gender = gender;
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                }
                string sex = null;
                if (gender == "Women")
                {
                    sex = "female";
                }
                if (gender == "Men")
                {
                    sex = "male";
                }

                var items = products;
                switch (category)
                {
                    case "All":
                        products = from p in _context.Items select p;
                        products = products.Where(o => o.gender == sex);
                        ViewBag.currentBrands = products.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Blazer Jackets":
                        products = products.Where(o => (o.long_description.Contains("Blazer jacket") || o.long_description.Contains("blazer jacket")) && o.gender == "male");
                        ViewBag.currentCategory = "Blazer Jackets";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Blazer jacket") || o.long_description.Contains("blazer jacket"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Briefcases":
                        products = products.Where(o => o.long_description.Contains("Briefcase") || o.long_description.Contains("briefcase"));
                        ViewBag.currentCategory = "Briefcases";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Briefcase") || o.long_description.Contains("briefcase"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Laptop Bags":
                        products = products.Where(o => o.long_description.Contains("Laptop bag") || o.long_description.Contains("laptop bag"));
                        ViewBag.currentCategory = "Laptop Bags";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Laptop bag") || o.long_description.Contains("laptop bag"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Shirts":
                        products = products.Where(o => (o.long_description.Contains("Shirt") || o.long_description.Contains("shirt")) && !(o.long_description.Contains("T-shirt")) && o.gender == "male");
                        ViewBag.currentCategory = "Shirts";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Shirt") || o.long_description.Contains("shirt")) && !(o.long_description.Contains("T-shirt")) && o.gender == "male");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "T-shirts":
                        products = products.Where(o => o.long_description.Contains("T-shirt") && o.gender == "male");
                        ViewBag.currentCategory = "T-shirts";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("T-shirt") && o.gender == "male");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suit Jackets":
                        products = products.Where(o => (o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket")));
                        ViewBag.currentCategory = "Suit Jackets";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket")));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suit Pants":
                        products = products.Where(o => o.long_description.Contains("Suit pants") || o.long_description.Contains("suit pants") || o.long_description.Contains("Suit trousers") || o.long_description.Contains("suit trousers"));
                        ViewBag.currentCategory = "Suit Pants";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Suit pants") || o.long_description.Contains("suit pants") || o.long_description.Contains("Suit trousers") || o.long_description.Contains("suit trousers"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suits":
                        products = products.Where(o => (o.long_description.Contains("Suit") || o.long_description.Contains("Suit")) && !(o.long_description.Contains("Suit Pants") || o.long_description.Contains("suit pants")) && !(o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket") || o.long_description.Contains("Suit trousers") || o.long_description.Contains("suit trousers")));
                        ViewBag.currentCategory = "Suits";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Suit") || o.long_description.Contains("Suit")) && !(o.long_description.Contains("Suit Pants") || o.long_description.Contains("suit pants")) && !(o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket") || o.long_description.Contains("Suit trousers") || o.long_description.Contains("suit trousers")));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Short Coats":
                        products = products.Where(o => o.long_description.Contains("Short coat") || o.long_description.Contains("short coat"));
                        ViewBag.currentCategory = "Short Coats";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Short coat") || o.long_description.Contains("short coat"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Waist Coats":
                        products = products.Where(o => o.long_description.Contains("Waistcoat") || o.long_description.Contains("waistcoat"));
                        ViewBag.currentCategory = "Waist Coats";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Waistcoat") || o.long_description.Contains("waistcoat"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Blazers":
                        products = products.Where(o => (o.long_description.Contains("Blazer") || o.long_description.Contains("blazer")) && o.gender == "female");
                        ViewBag.currentCategory = "Blazer";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Blazer") || o.long_description.Contains("blazer")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Blouses":
                        products = products.Where(o => o.long_description.Contains("Blouse") || o.long_description.Contains("blouse"));
                        ViewBag.currentCategory = "Blouse";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Blouse") || o.long_description.Contains("blouse")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Boots":
                        products = products.Where(o => o.long_description.Contains("Boots") || o.long_description.Contains("boots"));
                        ViewBag.currentCategory = "Boots";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Boots") || o.long_description.Contains("boots")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Coats":
                        products = products.Where(o => o.long_description.Contains("Coat") || o.long_description.Contains("coat"));
                        ViewBag.currentCategory = "Coat";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Coat") || o.long_description.Contains("coat")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Cardigans":
                        products = products.Where(o => o.long_description.Contains("Cardigan") || o.long_description.Contains("cardigan"));
                        ViewBag.currentCategory = "Cardigan";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Cardigan") || o.long_description.Contains("cardigan")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Dresses":
                        products = products.Where(o => (o.long_description.Contains("Dress") || o.long_description.Contains("dress")) && !o.long_description.Contains("shirt"));
                        ViewBag.currentCategory = "Dress";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Dress") || o.long_description.Contains("dress")) && !o.long_description.Contains("shirt"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Heels":
                        products = products.Where(o => o.long_description.Contains("Heels") || o.long_description.Contains("heels"));
                        ViewBag.currentCategory = "Heels";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Heels") || o.long_description.Contains("heels")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Jumpers":
                        products = products.Where(o => o.long_description.Contains("Jumper") || o.long_description.Contains("jumper"));
                        ViewBag.currentCategory = "Jumper";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Jumper") || o.long_description.Contains("jumper")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Skirts":
                        products = products.Where(o => o.long_description.Contains("Skirt") || o.long_description.Contains("skirt"));
                        ViewBag.currentCategory = "Skirt";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Skirt") || o.long_description.Contains("skirt")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Trousers":
                        products = products.Where(o => o.long_description.Contains("Trousers") || o.long_description.Contains("trousers"));
                        ViewBag.currentCategory = "Trousers";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Trousers") || o.long_description.Contains("trousers")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                }
                
                if (sortBy == "Recommended" && !String.IsNullOrEmpty(currentSort) && SortByRemember)
                {
                    switch (currentSort)
                    {
                        case "Name A - Z":
                            sortBy = "Name A - Z";
                            break;
                        case "Name Z- A":
                            sortBy = "Name Z- A";
                            break;
                        case "Price Low - High":
                            sortBy = "Price Low - High";
                            break;
                        case "Price High - Low":
                            sortBy = "Price High - Low";
                            break;

                            //<option value="Name A - Z">Name A - Z</option>
                            //<option value="Name Z- A">Name Z- A</option>
                            //<option value="Price Low - High">Price Low - High</option>
                            //<option value="Price High - Low">Price High - Low</option>
                    }
                }
                switch (sortBy)
                {
                    case "Price Low - High":
                        ViewBag.currentSort = "Price Low - High";
                        products = products.OrderBy(s => s.price);
                        break;
                    case "Price High - Low":
                        ViewBag.currentSort = "Price High - Low";
                        products = products.OrderByDescending(s => s.price);
                        break;
                    case "Name A - Z":
                        ViewBag.currentSort = "Name A - Z";
                        products = products.OrderBy(s => s.description);
                        break;
                    case "Name Z- A":
                        ViewBag.currentSort = "Name Z- A";
                        products = products.OrderByDescending(s => s.description);
                        break;
                }
                ViewData["searchString"] = searchString;

            }

            //ping pong session
            ViewBag.ColorList = clrList;

            ViewBag.CurrentColor = currentColor;

            ViewBag.Colorschecked = ColorBox.ToList();
            ViewBag.Brandschecked = BrandBox.ToList();
            //search stuff

            ViewData["SearchString"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            //als String niet leeg is dan wordt alles hieronder uitgevoerd
            {
                products = products.Where(s => s.description.ToUpper().Contains(searchString.ToUpper()) || s.long_description.ToLower().Contains(searchString.ToLower()));
                // vul searchstring in
                ViewBag.Message = searchString;
                //spreekt voorzich
                ViewBag.Count = products.Where(s => s.description.ToUpper().Contains(searchString.ToUpper()) || s.long_description.ToLower().Contains(searchString.ToLower())).Count();
                //spreekt voorzich
                var zero = products.Where(s => s.description.ToUpper().Contains(searchString.ToUpper()) || s.long_description.ToLower().Contains(searchString.ToLower())).Count() == 0;
                // zero is waar alleen als er 0 producten resulteren uit de searchstring query
                if (zero)
                {
                    ViewBag.Message = null;
                    var itemeru = from p in _context.Items select p;
                    return View(await itemeru.ToListAsync());
                }
                else
                {
                    int PageCount = products.Count() / 21;
                    if (products.Count() % 21 > 0) { PageCount++; }
                    IList<int> Pager = new List<int>();
                    for (int q = 1; q <= PageCount; q++)
                    { Pager.Add(q); }
                    ViewBag.Pager = Pager;
                    ViewBag.PageCount = PageCount;
                    ViewBag.BrandCount = products.Select(o => o.description).Distinct().Count();

                    return View(await products.ToListAsync());
                }
            }
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

        public async Task<IActionResult> Men(string category, string maxPrice, string minPrice, string currentCategory, string currentBrands, IEnumerable<string> BrandBox, IEnumerable<string> ColorBox, string sortBy, string currentSort)
        {
            // Incoming information from the form.
            ViewData["BrandBox"] = BrandBox;
            ViewData["maxPrice"] = maxPrice;
            ViewData["minPrice"] = minPrice;
            ViewData["ColorBox"] = ColorBox;
            ViewData["category"] = category;
            ViewData["currentCategory"] = currentCategory;
            ViewData["currentBrands"] = currentBrands;
            ViewData["sortBy"] = sortBy;
            ViewData["currentSort"] = currentSort;


            if (currentCategory != null)
            {
                ViewBag.currentCategory = currentCategory;
            }

            if (minPrice == null)
            {
                ViewBag.miniPrice = 0;
            }
            else
            {
                ViewBag.miniPrice = minPrice;
            }
            if (maxPrice == null)
            {
                ViewBag.maxiPrice = 1500;
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
            clrList.Add("green");
            clrList.Add("yellow");
            clrList.Add("black");
            clrList.Add("white");
            clrList.Add("navy");
            clrList.Add("grey");
            clrList.Add("brown");
            

            ViewBag.ColorList = clrList;
            // Retrieve the products from the database.
            var products = from p in _context.Items select p;
            products = products.Where(o => o.gender == "male");
            ViewBag.currentSort = "Recommended";
            bool SortByRemember = String.IsNullOrEmpty(sortBy);
            if (String.IsNullOrEmpty(sortBy)) { sortBy = "Recommended"; }
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
                    ViewBag.currentCategory = ViewData["currentCategory"];
                    if (String.IsNullOrEmpty(category)) { category = currentCategory; }
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
                var items = products;
                switch (category)
                {
                    case "All":
                        products = from p in _context.Items select p;
                        break;
                    case "Blazer Jackets":
                        products = products.Where(o => o.long_description.Contains("Blazer jacket") || o.long_description.Contains("blazer jacket"));
                        ViewBag.currentCategory = "Blazer Jackets";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Blazer jacket") || o.long_description.Contains("blazer jacket"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Briefcases":
                        products = products.Where(o => o.long_description.Contains("Briefcase") || o.long_description.Contains("briefcase"));
                        ViewBag.currentCategory = "Briefcases";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Briefcase") || o.long_description.Contains("briefcase"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Laptop Bags":
                        products = products.Where(o => o.long_description.Contains("Laptop bag") || o.long_description.Contains("laptop bag"));
                        ViewBag.currentCategory = "Laptop Bags";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Laptop bag") || o.long_description.Contains("laptop bag"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Shirts":
                        products = products.Where(o => o.long_description.Contains("Shirt") || o.long_description.Contains("shirt") && !(o.long_description.Contains("T-shirt")));
                        ViewBag.currentCategory = "Shirts";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Shirt") || o.long_description.Contains("shirt") && !(o.long_description.Contains("T-shirt")));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "T-shirts":
                        products = products.Where(o => o.long_description.Contains("T-shirt"));
                        ViewBag.currentCategory = "T-shirts";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("T-shirt"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suit Jackets":
                        products = products.Where(o => (o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket")));
                        ViewBag.currentCategory = "Suit Jackets";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket")));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suit Pants":
                        products = products.Where(o => o.long_description.Contains("Suit pants") || o.long_description.Contains("suit pants") || o.long_description.Contains("Suit trousers") || o.long_description.Contains("suit trousers"));
                        ViewBag.currentCategory = "Suit Pants";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Suit pants") || o.long_description.Contains("suit pants") || o.long_description.Contains("Suit trousers") || o.long_description.Contains("suit trousers"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Suits":
                        products = products.Where(o => (o.long_description.Contains("Suit") || o.long_description.Contains("Suit")) && !(o.long_description.Contains("Suit Pants") || o.long_description.Contains("suit pants")) && !(o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket") || o.long_description.Contains("Suit trousers") || o.long_description.Contains("suit trousers")));
                        ViewBag.currentCategory = "Suits";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Suit") || o.long_description.Contains("Suit")) && !(o.long_description.Contains("Suit Pants") || o.long_description.Contains("suit pants")) && !(o.long_description.Contains("Suit Jackets") || o.long_description.Contains("suit jacket") || o.long_description.Contains("Suit trousers") || o.long_description.Contains("suit trousers")));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Short Coats":
                        products = products.Where(o => o.long_description.Contains("Short coat") || o.long_description.Contains("short coat"));
                        ViewBag.currentCategory = "Short Coats";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Short coat") || o.long_description.Contains("short coat"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Waist Coats":
                        products = products.Where(o => o.long_description.Contains("Waistcoat") || o.long_description.Contains("waistcoat"));
                        ViewBag.currentCategory = "Waist Coats";
                        items = from p in _context.Items select p;
                        items = items.Where(o => o.long_description.Contains("Waistcoat") || o.long_description.Contains("waistcoat"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                }
                if (sortBy == "Recommended" && SortByRemember && !String.IsNullOrEmpty(currentSort))
                {
                    switch (currentSort)
                    {
                        case "Name A - Z":
                            sortBy = "Name A - Z";
                            break;
                        case "Name Z- A":
                            sortBy = "Name Z- A";
                            break;
                        case "Price Low - High":
                            sortBy = "Price Low - High";
                            break;
                        case "Price High - Low":
                            sortBy = "Price High - Low";
                            break;

                            //<option value="Name A - Z">Name A - Z</option>
                            //<option value="Name Z- A">Name Z- A</option>
                            //<option value="Price Low - High">Price Low - High</option>
                            //<option value="Price High - Low">Price High - Low</option>
                    }
                }
                
            }
            switch (sortBy)
            {
                default:
                    products = products.OrderBy(s => s.id);
                    break;
                case "Price Low - High":
                    ViewBag.currentSort = "Price Low - High";
                    products = products.OrderBy(s => s.price);
                    break;
                case "Price High - Low":
                    ViewBag.currentSort = "Price High - Low";
                    products = products.OrderByDescending(s => s.price);
                    break;
                case "Name A - Z":
                    ViewBag.currentSort = "Name A - Z";
                    products = products.OrderBy(s => s.description);
                    break;
                case "Name Z- A":
                    ViewBag.currentSort = "Name Z- A";
                    products = products.OrderByDescending(s => s.description);
                    break;
            }



            int PageCount = products.Count() / 21;

            ViewBag.Colorschecked = ColorBox.ToList();
            ViewBag.Brandschecked = BrandBox.ToList();
            

            ViewBag.PageCount = PageCount;

            ViewBag.BrandCount = products.Select(o => o.description).Distinct().Count();


            return View(await products.ToListAsync());
        }
        
        public async Task<IActionResult> Women(string category, string maxPrice, string minPrice, string currentCategory, string currentBrands, IEnumerable<string> BrandBox, IEnumerable<string> ColorBox, string sortBy, string currentSort)
        {
            // Incoming information from the form.
            ViewData["BrandBox"] = BrandBox;
            ViewData["maxPrice"] = maxPrice;
            ViewData["minPrice"] = minPrice;
            ViewData["ColorBox"] = ColorBox;
            ViewData["category"] = category;
            ViewData["currentCategory"] = currentCategory;
            ViewData["currentBrands"] = currentBrands;
            ViewData["sortBy"] = sortBy;
            ViewData["currentSort"] = currentSort;

            if (currentCategory != null)
            {
                ViewBag.currentCategory = currentCategory;
            }

            if (minPrice == null)
            {
                ViewBag.miniPrice = 0;
            }
            else
            {
                ViewBag.miniPrice = minPrice;
            }
            if (maxPrice == null)
            {
                ViewBag.maxiPrice = 1500;
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
            clrList.Add("black");
            clrList.Add("white");
            clrList.Add("grey");
            clrList.Add("navy");
            clrList.Add("brown");

            ViewBag.ColorList = clrList;
            ViewBag.currentSort = "Recommended";
            bool SortByRemember = String.IsNullOrEmpty(sortBy);
            if (String.IsNullOrEmpty(sortBy)) { sortBy = "Recommended"; }
            // Retrieve the products from the database.
            var products = from p in _context.Items
                           select p;
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
                var items = products;
                switch (category)
                {
                    case "All":
                        products = from p in _context.Items select p;
                        break;
                    case "Blazers":
                        products = products.Where(o => o.long_description.Contains("Blazer") || o.long_description.Contains("blazer") );
                        ViewBag.currentCategory = "Blazer";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Blazer") || o.long_description.Contains("blazer")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Blouses":
                        products = products.Where(o => o.long_description.Contains("Blouse") || o.long_description.Contains("blouse"));
                        ViewBag.currentCategory = "Blouse";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Blouse") || o.long_description.Contains("blouse")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Boots":
                        products = products.Where(o => o.long_description.Contains("Boots") || o.long_description.Contains("boots"));
                        ViewBag.currentCategory = "Boots";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Boots") || o.long_description.Contains("boots")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Coats":
                        products = products.Where(o => o.long_description.Contains("Coat") || o.long_description.Contains("coat"));
                        ViewBag.currentCategory = "Coat";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Coat") || o.long_description.Contains("coat")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Cardigans":
                        products = products.Where(o => o.long_description.Contains("Cardigan") || o.long_description.Contains("cardigan"));
                        ViewBag.currentCategory = "Cardigan";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Cardigan") || o.long_description.Contains("cardigan")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Dresses":
                        products = products.Where(o => (o.long_description.Contains("Dress") || o.long_description.Contains("dress")) && !o.long_description.Contains("shirt"));
                        ViewBag.currentCategory = "Dress";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Dress") || o.long_description.Contains("dress")) && !o.long_description.Contains("shirt"));
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Heels":
                        products = products.Where(o => o.long_description.Contains("Heels") || o.long_description.Contains("heels"));
                        ViewBag.currentCategory = "Heels";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Heels") || o.long_description.Contains("heels")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Jumpers":
                        products = products.Where(o => o.long_description.Contains("Jumper") || o.long_description.Contains("jumper"));
                        ViewBag.currentCategory = "Jumper";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Jumper") || o.long_description.Contains("jumper")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Skirts":
                        products = products.Where(o => o.long_description.Contains("Skirt") || o.long_description.Contains("skirt"));
                        ViewBag.currentCategory = "Skirt";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Skirt") || o.long_description.Contains("skirt")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                    case "Trousers":
                        products = products.Where(o => o.long_description.Contains("Trousers") || o.long_description.Contains("trousers"));
                        ViewBag.currentCategory = "Trousers";
                        items = from p in _context.Items select p;
                        items = items.Where(o => (o.long_description.Contains("Trousers") || o.long_description.Contains("trousers")) && o.gender == "female");
                        ViewBag.currentBrands = items.Select(o => o.description).Distinct().ToList();
                        break;
                }
            }

            if (sortBy == "Recommended" && !String.IsNullOrEmpty(currentSort) && SortByRemember)
            {
                switch (currentSort)
                {
                    case "Name A - Z":
                        sortBy = "Name A - Z";
                        break;
                    case "Name Z- A":
                        sortBy = "Name Z- A";
                        break;
                    case "Price Low - High":
                        sortBy = "Price Low - High";
                        break;
                    case "Price High - Low":
                        sortBy = "Price High - Low";
                        break;

                        //<option value="Name A - Z">Name A - Z</option>
                        //<option value="Name Z- A">Name Z- A</option>
                        //<option value="Price Low - High">Price Low - High</option>
                        //<option value="Price High - Low">Price High - Low</option>
                }
            }
            switch (sortBy)
            {
                default:
                    products = products.OrderBy(s => s.id);
                    break;
                case "Price Low - High":
                    ViewBag.currentSort = "Price Low - High";
                    products = products.OrderBy(s => s.price);
                    break;
                case "Price High - Low":
                    ViewBag.currentSort = "Price High - Low";
                    products = products.OrderByDescending(s => s.price);
                    break;
                case "Name A - Z":
                    ViewBag.currentSort = "Name A - Z";
                    products = products.OrderBy(s => s.description);
                    break;
                case "Name Z- A":
                    ViewBag.currentSort = "Name Z- A";
                    products = products.OrderByDescending(s => s.description);
                    break;
            }
            int PageCount = 1;
            PageCount = products.Count() / 21;

            
            ViewBag.Colorschecked = ColorBox.ToList();
            ViewBag.Brandschecked = BrandBox.ToList();

            ViewBag.PageCount = PageCount;

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
