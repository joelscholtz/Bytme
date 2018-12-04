using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bytme.Helpers;
using bytme.Models;
using System.Net.Mail;
using System.Net;
using bytme.Data;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Identity;

namespace bytme.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public int CheckIfOrderExists()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            OrderMain orderMain = _context.OrderMains.Where(o => o.user_id == userId && o.ordstatus_id == 1).FirstOrDefault();
            if (orderMain != null)
            {
                return orderMain.id;
            }
            else
            {
                return 0;
            }

        }

        public int CreateOrder()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            OrderMain orderMain = new OrderMain();
            orderMain.user_id = userId;
            orderMain.ordstatus_id = 1;
            orderMain.dt_created = DateTime.Now;
            _context.Add(orderMain);
            _context.SaveChanges();

            return orderMain.id;
        }

        private int IsExist(int item_id)
        {
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            for(int i = 0; i < cart.Count; i++)
            {
                if (cart[i].item_id.Equals(item_id))
                {
                    return i;
                }
            }
            return -1;
        }

        public IActionResult Remove(int item_id)
        {
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            int index = IsExist(item_id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult SaveItemsSession(int item_id, int qty)
        {
            var CheckItem = _context.Items.Where(o => o.id == item_id).FirstOrDefault();

            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart") == null)
            {
                List<Product> cart = new List<Product>();
                cart.Add(new Product
                {
                    item_id = CheckItem.id,
                    description = CheckItem.description,
                    category_id = CheckItem.category_id,
                    long_description = CheckItem.long_description,
                    gender = CheckItem.gender,
                    issales = CheckItem.issales,
                    photo_url = CheckItem.photo_url,
                    price = CheckItem.price,
                    quantity = qty,
                    stock = CheckItem.quantity,
                    size = CheckItem.size
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
                int index = IsExist(item_id);
                if(index != -1)
                {
                    if (cart[index].quantity < cart[index].stock)
                    {
                        cart[index].quantity += qty;
                        if (cart[index].quantity > cart[index].stock)
                        {
                            cart[index].quantity = cart[index].stock;
                        }
                    }
                    else { }
                }
                else
                {
                    cart.Add(new Product
                    {
                        item_id = CheckItem.id,
                        description = CheckItem.description,
                        category_id = CheckItem.category_id,
                        long_description = CheckItem.long_description,
                        gender = CheckItem.gender,
                        issales = CheckItem.issales,
                        photo_url = CheckItem.photo_url,
                        price = CheckItem.price,
                        quantity = qty,
                        stock = CheckItem.quantity,
                        size = CheckItem.size
                    });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SaveItemInShoppingCart(int item_id, int qty)
        {
            int _order_id;
            int order_id = CheckIfOrderExists();
            if(order_id != 0)
            {
                _order_id = order_id;
            }
            else
            {
                _order_id = CreateOrder();
            }

            var CheckItemId = _context.OrderLines.Where(o => o.item_id == item_id && o.order_id == _order_id).FirstOrDefault();
            var CheckItemStock = _context.Items.Where(o => o.id == item_id).FirstOrDefault();

            if (CheckItemId != null)
            {
                if(CheckItemId.qty < CheckItemStock.quantity)
                {
                    CheckItemId.qty += qty;
                    if(CheckItemId.qty > CheckItemStock.quantity)
                    {
                        CheckItemId.qty = CheckItemStock.quantity;
                    }
                    _context.Update(CheckItemId);
                    _context.SaveChanges();
                }
                else {}
            }
            else
            {
                OrderLines orderLines = new OrderLines();
                orderLines.item_id = item_id;
                orderLines.order_id = _order_id;
                orderLines.qty = qty;
                _context.Add(orderLines);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult DeleteItemFromShoppingCart(int orderline_id)
        {
            var result = (from OrderLines in _context.OrderLines
                          where OrderLines.id == orderline_id
                          select OrderLines).FirstOrDefault();

            if(result != null)
            {
                _context.OrderLines.Remove(result);
                _context.SaveChanges();
            }
            else
            {
                ViewBag.ErrorMessage = "There was an error.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateQuantityInShoppingCart(int orderline_id, int qty)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = _context.OrderMains.Where(o => o.user_id == userId && o.ordstatus_id == 1).FirstOrDefault();

            var listOfItems = _context.OrderLines.Where(o => o.order_id == orders.id && o.id == orderline_id).FirstOrDefault();

            listOfItems.qty = qty;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult UpdateQuantityInShoppingCartSession(int item_id, int qty)
        {
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");

            cart[item_id].quantity = qty;

            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            return RedirectToAction(nameof(Index));
        }

        public int CountItemsInShoppingCart(int order_id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = (from orderlines in _context.OrderLines
                          join items in _context.Items on orderlines.item_id equals items.id
                          join ordermains in _context.OrderMains on orderlines.order_id equals ordermains.id
                          where orderlines.order_id == order_id
                          select orderlines).Count();

            return result;
        }

        public int TotalCount()
        {
            int order_id = CheckIfOrderExists();
            int count = CountItemsInShoppingCart(order_id);
            ViewBag.count = count;

            return ViewBag.count;
        }

        public int TotalCountSession()
        {
            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart") == null)
            {
                int countsession = 0;
                ViewBag.countsession = countsession;

                return ViewBag.countsession;
            }
            else
            {
                List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
                int countsession = cart.Count();
                ViewBag.countsession = countsession;

                return ViewBag.countsession;
            }
        }

        public void CreateOrderHistory(OrderMain m)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            OrderHistory history = new OrderHistory();
            var result = (from orderline in _context.OrderLines
                          join ordermain in _context.OrderMains on orderline.order_id equals ordermain.id
                          join item in _context.Items on orderline.item_id equals item.id
                          join user in _context.UserModels on ordermain.user_id equals user.Id
                          where orderline.order_id == m.id && UserId == m.user_id
                          select new OrderHistory
                          {
                              item_description = item.description,
                              ord_id = orderline.order_id,
                              oderline_id = orderline.id,
                              user_id = UserId,
                              price_payed = item.price,
                              qty_bought = orderline.qty,
                              street = user.street,
                              streetnumber = user.streetnumber,
                              city = user.city,
                              zipcode = user.zipcode,
                              dt_created = DateTime.Now,
                              
                          }).ToList();

            foreach(var r in result)
            {
                _context.Add(r);
                _context.SaveChanges();
            }
        }

        public void DecreaseStock(int item_id, int qty)
        {
            var item = (from i in _context.Items
                        where i.id == item_id
                        select i).FirstOrDefault();

            var orderline = (from o in _context.OrderLines
                             from m in _context.OrderMains
                             where o.order_id == m.id && o.item_id == item_id
                             select o).FirstOrDefault();

            item.quantity = item.quantity - qty;

            _context.Items.Update(item);
            _context.SaveChanges();
        }

        public IActionResult SessionToAccount()
        {
            Boolean decreased = false;
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");

            int _order_id;
            int order_id = CheckIfOrderExists();
            if (order_id != 0)
            {
                _order_id = order_id;
            }
            else
            {
                _order_id = CreateOrder();
            }

            if (decreased == false)
            {
                foreach(var item in cart)
                {
                    var CheckItemId = _context.OrderLines.Where(o => o.item_id == item.item_id && o.order_id == _order_id).FirstOrDefault();
                    var CheckItemStock = _context.Items.Where(o => o.id == item.item_id).FirstOrDefault();

                    if (CheckItemId != null)
                    {
                        if (CheckItemId.qty < CheckItemStock.quantity)
                        {
                            CheckItemId.qty += item.quantity;
                            if(CheckItemId.qty > CheckItemStock.quantity)
                            {
                                CheckItemId.qty = CheckItemStock.quantity;
                            }
                            _context.Update(CheckItemId);
                            _context.SaveChanges();
                        }
                        else { }
                    }
                    else
                    {
                        OrderLines orderLines = new OrderLines();
                        orderLines.item_id = item.item_id;
                        orderLines.order_id = _order_id;
                        orderLines.qty = item.quantity;
                        _context.Add(orderLines);
                        _context.SaveChanges();
                    }
                }
            }
            decreased = true;
            if(decreased == true)
            {
                foreach(var item in cart)
                {
                    Remove(item.item_id);
                }
            }
            return RedirectToAction("index", "ShoppingCart");
        }

        public IActionResult ConfirmOrderSession()
        {
            var user = SessionHelper.GetObjectFromJson<List<UserModel>>(HttpContext.Session, "user");
            Boolean decreased = false;
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            if (decreased == false)
            {
                foreach(var item in cart)
                {
                    DecreaseStock(item.item_id, item.quantity);
                }
            }
            decreased = true;
            if (decreased == true)
            {
                foreach(var item in cart)
                {
                    Remove(item.item_id);
                }
            }

            //send email |
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("sayoswebshop@gmail.com", "PonyParkSlagHaren1234");

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("sayoswebshop@gmail.com");
            mailMessage.To.Add(user[0].Email);
            mailMessage.Subject = "Order Sayos Webshop";
            string fullname = user[0].name + " " + user[0].surname;

            mailMessage.Body = " <div style='width:95%;'> " +
                                "<h3><center>Address:</center></h3>" +
                                "<h3><center>" + user[0].street + " " + user[0].streetnumber + "," + "</center></h3>" +
                                "<h3><center>" + user[0].zipcode + ", " + user[0].city + "</center></h3>";
            mailMessage.Body += "<br><br><h3>Dear " + fullname + " thanks for your order!</h3>";
            mailMessage.Body +=
                "<h3>Here is an overview of the things you ordered:</h3> <br><br> " +
                " <table style='font-family: arial,sans-serif;border-collapse: collapse;width:95%; '>" +
                " <tr> " +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Picture:</th>" +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Description:</th>" +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Amount:</th>" +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Price:</th>" +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Subtotal:</th>" +
                " </tr>";

            float totalPrice = 0f;
            float sendcost = 2.95f;
            foreach (var item in cart)
            {
                mailMessage.Body +=
                //mailMessage.Body += "<td><img style='width:300px;' src='" + photo_url + item.Itms.photo_url + "'/></td>";
                " <tr>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'><img style='width:200px; height:250px' src='" + item.photo_url + "'/></td>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'>" + item.description + "</td>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'>" + item.quantity + "</td>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'>" + item.price + " euro</td>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'>" + item.price * item.quantity + " euro</td>" +
                "</tr>";
                totalPrice += (item.price * item.quantity);
            }
            if (totalPrice < 100)
            {
                totalPrice += sendcost;
                mailMessage.Body += " <tr> " +
                                   " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                   " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                   " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                   " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                   " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;'>Shipping costs: " + sendcost + " euro</th>" +
                                   " </tr>";

                mailMessage.Body += " <tr> " +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;'>Total: " + totalPrice + " euro</th>" +
                                    " </tr>";
            }
            else
            {
                mailMessage.Body += " <tr> " +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;'>Total: " + totalPrice + " euro</th>" +
                                    " </tr>";
            }
            mailMessage.Body += "</table>";
            mailMessage.Body += "<br><h3>If you have any questions you can mail us via the contact page on our webshop.</h3>" +
                                "<br><h3>Have a nice day!</h3>" +
                                "</div>";

            client.Send(mailMessage);
            user.RemoveAt(0);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "user", user);
            return RedirectToAction("index", "Home");
        }

        public IActionResult ConfirmOrder()
        {
            Boolean decreased = false;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _context.UserModels.Where(u => u.Id == userId).FirstOrDefault();
            var orders = _context.OrderMains.Where(o => o.user_id == userId && o.ordstatus_id == 1).FirstOrDefault();
            var ListOfItems = _context.OrderLines.Where(o => o.order_id == orders.id);

            if (decreased == false)
            {
                foreach (var l in ListOfItems)
                {
                    if(l.item_id != 0)
                    {
                        DecreaseStock(l.item_id, l.qty);
                    }
                }
            }
            decreased = true;
            if (decreased == true)
            {
                orders.ordstatus_id = 2;
                _context.Update(orders);
                _context.SaveChanges();
                CreateOrderHistory(orders);
            }
            var Listpurchitems = (from os in _context.OrderHistories
                                  join ordline in _context.OrderLines on os.oderline_id equals ordline.id
                                  join itm in _context.Items on ordline.item_id equals itm.id
                                  where os.ord_id == orders.id && os.user_id == userId
                                  select new
                                  {
                                      Item = itm,
                                      OrderLines = ordline,
                                      OrderHistory = os
                                  }).ToList();

            //send email |
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("sayoswebshop@gmail.com", "PonyParkSlagHaren1234");
            
            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("sayoswebshop@gmail.com");
            mailMessage.To.Add(result.Email);
            mailMessage.Subject = "Order " + orders.id;
            string fullname = result.name + " " + result.surname;

            mailMessage.Body = " <div style='width:95%;'> " +
                                "<h3><center>Address:</center></h3>" +
                                "<h3><center>" + result.street + " " + result.streetnumber + "," + "</center></h3>" +
                                "<h3><center>" + result.zipcode + ", " + result.city + "</center></h3>";
            mailMessage.Body += "<br><br><h3>Dear " + fullname + " thanks for your order!</h3>";
            mailMessage.Body +=
                "<h3>Here is an overview of the things you ordered:</h3> <br><br> " +
                " <table style='font-family: arial,sans-serif;border-collapse: collapse;width:95%; '>" +
                " <tr> " +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Picture:</th>" +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Description:</th>" +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Amount:</th>" +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Price:</th>" +
                " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;font-size: 10pt;'>Subtotal:</th>" +
                " </tr>";

            float totalPrice = 0f;
            float sendcost = 2.95f;
            foreach (var item in Listpurchitems)
            {
                mailMessage.Body +=
                //mailMessage.Body += "<td><img style='width:300px;' src='" + photo_url + item.Itms.photo_url + "'/></td>";
                " <tr>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'><img style='width:200px; height:250px' src='" + item.Item.photo_url + "'/></td>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'>" + item.OrderHistory.item_description + "</td>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'>" + item.OrderHistory.qty_bought + "</td>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'>" + item.OrderHistory.price_payed + " euro</td>" +
                " <td style='border: 0px solid #dddddd;text-align: center; padding:8px;'>" + item.OrderHistory.price_payed * item.OrderHistory.qty_bought + " euro</td>" +
                "</tr>";
                totalPrice += (item.OrderHistory.price_payed * item.OrderHistory.qty_bought);
            }
            if (totalPrice < 100)
            {
                totalPrice += sendcost;
                mailMessage.Body += " <tr> " +
                                   " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                   " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                   " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                   " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                   " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;'>Shipping costs: " + sendcost + " euro</th>" +
                                   " </tr>";

                mailMessage.Body += " <tr> " +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;'>Total: " + totalPrice + " euro</th>" +
                                    " </tr>";
            }
            else
            {
                mailMessage.Body += " <tr> " +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: left; padding:8px;'></th>" +
                                    " <th style='border: 0px solid #dddddd;text-align: center; padding:8px;'>Total: " + totalPrice + " euro</th>" +
                                    " </tr>";
            }
            mailMessage.Body += "</table>";
            mailMessage.Body += "<br><h3>If you have any questions you can mail us via the contact page on our webshop.</h3>" +
                                "<br><h3>Have a nice day!</h3>" +
                                "</div>";

            client.Send(mailMessage);


            return RedirectToAction("index", "Home");
        }

        public ActionResult Confirmation(OrderMain m)
        {
            IQueryable<ShoppingCartModel> model = null;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _context.UserModels.Where(o => o.Id == userId).FirstOrDefault();
            int order_id = CheckIfOrderExists();
            if (order_id != 0)
            {
                model = from orderlines in _context.OrderLines
                        join items in _context.Items on orderlines.item_id equals items.id
                        join ordermains in _context.OrderMains on orderlines.order_id equals ordermains.id
                        where orderlines.order_id == order_id
                        orderby orderlines.id
                        select new ShoppingCartModel
                        {
                            description = items.description,
                            price = items.price,
                            qty = orderlines.qty,
                            orderline_id = orderlines.id,
                            photo_url = items.photo_url,
                            stock = items.quantity,
                            size = items.size,
                            long_description = items.long_description,
                            item_id = items.id,
                            order_id = order_id
                        };

                return View(model.ToList());
            }
            ViewBag.model_view = model;

            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            if(ViewBag.cart != null)
            {
                ViewBag.total = cart.Sum(item => item.price * item.quantity);
            }

            IQueryable<ShoppingCartModel> model = null;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _context.UserModels.Where(o => o.Id == userId).FirstOrDefault();
            float totalPrice = 0;
            float sendcost = 2.95f;
            int order_id = CheckIfOrderExists();
            if(order_id != 0)
            {
                model = from orderlines in _context.OrderLines
                        join items in _context.Items on orderlines.item_id equals items.id
                        join ordermains in _context.OrderMains on orderlines.order_id equals ordermains.id
                        where orderlines.order_id == order_id 
                        orderby orderlines.id
                        select new ShoppingCartModel
                        {
                            description = items.description,
                            price = items.price,
                            qty = orderlines.qty,
                            orderline_id = orderlines.id,
                            photo_url = items.photo_url,
                            stock = items.quantity,
                            size = items.size,
                            long_description = items.long_description,
                            item_id = items.id,
                            order_id = order_id
                        };

                if(model != null)
                {
                    if(model.Count() > 0)
                    {
                        foreach(ShoppingCartModel item in model)
                        {
                            item.subtotal = item.qty * item.price;
                            totalPrice += item.subtotal;
                        }
                        if(totalPrice < 100)
                        {
                            ViewBag.totalPrice = totalPrice + sendcost;;
                        }
                        else
                        {
                            ViewBag.totalPrice = totalPrice;
                        }
                    }
                }
                int count = CountItemsInShoppingCart(order_id);
                ViewBag.order_id = order_id;
                ViewBag.count = count;
                ViewBag.model_view = model;


                return View(model.ToList());
            }
            ViewBag.model_view = model;

            return View();
        }

        public IActionResult Checkout(OrderHistoyModel o)
        {
            var cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            if (ViewBag.cart != null)
            {
                ViewBag.total = cart.Sum(item => item.price * item.quantity);
            }
            if (ModelState.IsValid)
            {
                if (SessionHelper.GetObjectFromJson<List<UserModel>>(HttpContext.Session, "user") == null)
                {
                    List<UserModel> user = new List<UserModel>();
                    user.Add(new UserModel
                    {
                        Email = o.Email,
                        name = o.name,
                        surname = o.surname,
                        street = o.street,
                        streetnumber = o.streetnumber,
                        zipcode = o.zipcode,
                        city = o.city
                    });
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "user", user);
                }
                else
                {
                    List<UserModel> user = SessionHelper.GetObjectFromJson<List<UserModel>>(HttpContext.Session, "user");
                    user.RemoveAt(0);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "user", user);
                    user.Add(new UserModel
                    {
                        Email = o.Email,
                        name = o.name,
                        surname = o.surname,
                        street = o.street,
                        streetnumber = o.streetnumber,
                        zipcode = o.zipcode,
                        city = o.city
                    });
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "user", user);
                }
                return RedirectToAction("Payment");
            }
            return View();
    }

        public IActionResult Payment()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            if (ViewBag.cart != null)
            {
                ViewBag.total = cart.Sum(item => item.price * item.quantity);
            }
            return View();
        }

        public ActionResult OrderHistory()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = (from ordermain in _context.OrderMains
                          where ordermain.user_id == userId && ordermain.ordstatus_id == 2
                          orderby ordermain.id descending
                          select new OrderMain
                          {
                              id = ordermain.id,
                              ordstatus_id = ordermain.ordstatus_id,
                              dt_created = ordermain.dt_created
                          }
                          );

            return View(result.ToList());
        }

        public ActionResult OrderHistoryDetail(int order_id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = (from history in _context.OrderHistories
                          where history.user_id == userId && history.ord_id == order_id
                          orderby history.ord_id descending
                          select new OrderHistory
                          {
                              ord_id = history.ord_id,
                              price_payed = history.price_payed,
                              qty_bought = history.qty_bought,
                              item_description = history.item_description,
                              street = history.street,
                              streetnumber = history.streetnumber,
                              zipcode = history.zipcode,
                              city = history.city,
                              dt_created = history.dt_created
                          }
                              );

            return View(result.ToList());
        }

    }
}
