﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bytme.Models;
using System.Net.Mail;
using System.Net;
using bytme.Data;
using System.Security.Claims;

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

            OrderMain orderMain = _context.OrderMains.Where(o => o.user_id == userId).FirstOrDefault();
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
            _context.Add(orderMain);
            _context.SaveChanges();

            return orderMain.id;
        }

        public IActionResult SaveItemInShoppingCart(int item_id)
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
            if(CheckItemId != null)
            {
                CheckItemId.qty = CheckItemId.qty + 1;
                _context.Update(CheckItemId);
                _context.SaveChanges();
            }
            else
            {
                OrderLines orderLines = new OrderLines();
                orderLines.item_id = item_id;
                orderLines.order_id = _order_id;
                orderLines.qty = 1;
                _context.Add(orderLines);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult DeleteItemFromShoppingCart(int shoppingcart_id)
        {
            var result = (from shoppingcart in _context.ShoppingCartModels
                          where shoppingcart.id == shoppingcart_id
                          select shoppingcart).FirstOrDefault();
            if(result != null)
            {
                _context.ShoppingCartModels.Remove(result);
                _context.SaveChanges();
            }
            else
            {
                ViewBag.ErrorMessage = "There was an error.";
            }
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
        [HttpGet]
        public IActionResult Index()
        {
            IQueryable<ShoppingCartModel> model = null;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _context.UserModels.Where(o => o.Id == userId).FirstOrDefault();
            float totalPrice = 0;
            int order_id = CheckIfOrderExists();
            if(order_id != 0)
            {
                model = from orderlines in _context.OrderLines
                        join items in _context.Items on orderlines.item_id equals items.id
                        join ordermains in _context.OrderMains on orderlines.order_id equals ordermains.id
                        where orderlines.order_id == order_id
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
                            item.subtotal = item.quantity * item.price;
                            totalPrice += item.subtotal;
                        }
                        ViewBag.totalPrice = totalPrice;
                    }
                }
                int count = CountItemsInShoppingCart(order_id);
                ViewBag.count = count;
                ViewBag.model_view = model;

                return View(model.ToList());
            }
            ViewBag.model_view = model;

            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
