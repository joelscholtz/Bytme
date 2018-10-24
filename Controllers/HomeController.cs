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

        public IActionResult Men()
        {
            IEnumerable<Item> item = _context.Items.AsEnumerable();

            return View(item);
        }

        public IActionResult Women()
        {
            IEnumerable<Item> item = _context.Items.AsEnumerable();

            return View(item);
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
