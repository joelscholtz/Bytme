﻿using bytme.Data;
using bytme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
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

 
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Item newIt)
        {
            
            if(ModelState.IsValid)
            {
                var products = from p in _db.Items select p;
                Item product = products.OrderByDescending(o => o.id).FirstOrDefault();
                var max = product.id;

                newIt.id = Convert.ToInt32(max) + 1;
                var addIt = await _db.AddAsync(newIt);
                await _db.SaveChangesAsync();
                return RedirectToAction("Products");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(string id, UserModel userModel)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    if (user.name != userModel.name && userModel.name != null)
                    {
                        user.name = userModel.name;
                    }
                    if (user.surname != userModel.surname && userModel.surname != null)
                    {
                        user.surname = userModel.surname;
                    }
                    if (user.Email != userModel.Email && userModel.Email != null)
                    {
                        user.Email = userModel.Email;
                        //for later implementation calback url should be ://localhost:5001/Identity/Account/ConfirmEmail with user id and code
                        //___________________________________
                        //user.EmailConfirmed = false;
                        //// confirmation code + custom URL creation
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.("", "", new { Area = "Identity", Page = "/Account/ConfirmEmail", userId = user.Id, code = code  });
                        ////Url.Page("/Identity/Account/ConfirmEmail",
                        ////    pageHandler: ,
                        ////    values: new { userId = user.Id, code = code },
                        ////    protocol: Request.Scheme);

                        //// mailmessage creation
                        //MailMessage verifyMessage = new MailMessage();
                        //verifyMessage.IsBodyHtml = true;
                        //verifyMessage.From = new MailAddress("sayoswebshop@gmail.com");
                        //verifyMessage.To.Add(userModel.Email);
                        //verifyMessage.Subject = "Verify your email";
                        //verifyMessage.Body = $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";


                        //// SMTP details
                        //SmtpClient smtpClient = new SmtpClient();
                        //smtpClient.UseDefaultCredentials = false;
                        //smtpClient.Credentials = new NetworkCredential("sayoswebshop@gmail.com", "PonyParkSlagHaren1234");
                        //smtpClient.EnableSsl = true;
                        //smtpClient.Host = "smtp.gmail.com";
                        //smtpClient.Port = 587;

                        ////sends message to recipient
                        //smtpClient.Send(verifyMessage);

                    }
                    if (user.street != userModel.street && userModel.street != null)
                    {
                        user.street = userModel.street;
                    }
                    if (user.streetnumber != userModel.streetnumber && userModel.streetnumber != null)
                    {
                        user.streetnumber = userModel.streetnumber;
                    }
                    if (user.zipcode != userModel.zipcode && userModel.zipcode != null)
                    {
                        user.zipcode = userModel.zipcode;
                    }
                    if (user.city != userModel.city && userModel.city != null)
                    {
                        user.city = userModel.city;
                    }
               
                    var result = await _userManager.UpdateAsync(user);
                    var dbup = await _db.SaveChangesAsync();
                    return View(user);
                }
           
        
            }
            return View(userModel);
            
           
        }

        [HttpPost]
        public async Task<ActionResult> EditProduct(int Id, Item item)
        {
            var product = await _db.Items.FirstOrDefaultAsync(i => i.id == Id);
            if (ModelState.IsValid)
            {
                
                if (product != null)
                {
                    if (await TryUpdateModelAsync<Item>(product, "", p => p.description, p => p.category_id, p => p.gender, p => p.issales, p => p.long_description, p => p.photo_url, p => p.price, p => p.quantity, p => p.size)
                    ){
                        try
                        {
                            await _db.SaveChangesAsync();
                            return View(product);
                        }
                        catch (DbUpdateException)
                        {
                            ModelState.AddModelError(string.Empty, "An error occured while updating");
                        }

                    }
                    return View(product);
                }
                return View(product);
            }
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DropUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var role = _userManager.GetRolesAsync(user);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User could not be found and therefor could not be deleted");
                return RedirectToAction("Index");
            }

            var userRole = role.ToString();
            var removeRole = await _userManager.RemoveFromRoleAsync(user, userRole);
            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DropProduct(int id)
        {
            var item = await _db.Items.SingleOrDefaultAsync(p => p.id == id);

            var delete = _db.Items.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectToAction("Products");
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
                                        || item.gender.ToUpper().Contains(searchKeyP.ToUpper())).OrderBy(u => u.id);
            }
            var p_sorted = products.OrderBy(p => p.id);
            return View(p_sorted.ToList());
        }


        //statistics
        [Authorize(Roles = "Admin")]
        public IActionResult Statistics()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateProduct()
        {
            return View();
        }

        //update
        [Authorize(Roles = "Admin")]
        public IActionResult EditUser(string id)
        {
            //finds user sends all user data
            return View(_db.Users.Where(x => x.Id == id).FirstOrDefault());
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(int Id)
        {
            return View(_db.Items.FirstOrDefault(ip => ip.id == Id));
        }

        //delete
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(string Id)
        {
            if(Id == null)
            {
                RedirectToAction("Index", "Manage", null);
            }
            else
            {
                ViewBag.UserId = Id;            
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProduct(int Id)
        {
            Item OriginItem = _db.Items.FirstOrDefault(ip => ip.id == Id);
            var deleteItem = new Item();
            deleteItem.id = OriginItem.id;
            deleteItem.description = OriginItem.description;
            deleteItem.long_description = OriginItem.long_description;
            deleteItem.photo_url = OriginItem.photo_url;
            deleteItem.price = OriginItem.price;
            deleteItem.quantity = OriginItem.quantity;
            deleteItem.size = OriginItem.size;
            deleteItem.issales = OriginItem.issales;
            deleteItem.gender = OriginItem.gender;

            ViewBag.ItemCatId = OriginItem.category_id;

            return View(deleteItem);
        }
       
        
    }
}
