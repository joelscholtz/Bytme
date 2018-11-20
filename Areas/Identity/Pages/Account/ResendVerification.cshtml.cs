using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using bytme.Data;
using bytme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace bytme.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendVerificationModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ForgotPasswordModel> _logger;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ApplicationDbContext _context;

        public ResendVerificationModel(UserManager<UserModel> userManager, IEmailSender emailSender, ILogger<ForgotPasswordModel> logger, SignInManager<UserModel> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "E-mail has not been used in a register. If you think this is an error, please go to the contact form.");
                    return Page();
                }
                if (user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Account is already verified. If you think this is an error, please go to the contact form.");
                    return Page();
                }

                // confirmation code + custom URL creation
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = user.Id, code = code },
                    protocol: Request.Scheme);

                // mailmessage creation
                MailMessage verifyMessage = new MailMessage();
                verifyMessage.IsBodyHtml = true;
                verifyMessage.From = new MailAddress("sayoswebshop@gmail.com");
                verifyMessage.To.Add(Input.Email);
                verifyMessage.Subject = "Verify your email";
                verifyMessage.Body = $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

                // SMTP details
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("sayoswebshop@gmail.com", "PonyParkSlagHaren1234");
                smtpClient.EnableSsl = true;
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;

                //sends message to recipient
                smtpClient.Send(verifyMessage);
                ModelState.Clear();

                return RedirectToPage("./Verification");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}