using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using bytme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bytme.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailSender _emailSender;

        public CreateUserModel(
            UserManager<UserModel> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }


        [BindProperty]
        public AddedByAdminModel addByAdmin { get; set; }

        public class AddedByAdminModel
        {
            [Required]
            [EmailAddress]
            [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Street")]
            [RegularExpression(@"^[a-zA-Z -.]+$", ErrorMessage = "A street name can only contain letters. If you want to add a house number, do it in the next field.")]
            [StringLength(48, ErrorMessage = "The longest street name in the Netherlands is 48 characters.")]
            public string street { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "House Number")]
            [RegularExpression(@"^[0-9]{1,5}([a-z]{1,2})?$", ErrorMessage = "A house number starts with at least 1 digit. House number addition is possible.")]
            [StringLength(7, ErrorMessage = "The longest house number in the Netherlands is 7 characters.")]
            public string streetnumber { get; set; }

            [Required]
            [DataType(DataType.PostalCode)]
            [Display(Name = "Postal Code")]
            [RegularExpression(@"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$", ErrorMessage = "Invalid zip, for example: 1234AB")]
            public string zipcode { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "City")]
            [RegularExpression(@"^[a-zA-Z-]+$", ErrorMessage = "A city can only contain letters.")]
            [StringLength(28, ErrorMessage = "The longest place name in the Netherlands has 28 characters.")]
            public string city { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            [RegularExpression(@"^[a-zA-Z -]+$", ErrorMessage = "A name can only contain letters.")]
            [StringLength(100, ErrorMessage = "Invalid input. Maximum is 100 characters.")]
            public string name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Surname")]
            [RegularExpression(@"^[a-zA-Z -]+$", ErrorMessage = "A surname can only contain letters.")]
            [StringLength(100, ErrorMessage = "Invalid input. Maximum is 100 characters.")]
            public string surname { get; set; }

            public bool role { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

                var email = await _userManager.FindByEmailAsync(addByAdmin.Email);
                var user = new UserModel { dt_created = DateTime.Now, UserName = addByAdmin.Email, Email = addByAdmin.Email, street = addByAdmin.street, streetnumber = addByAdmin.streetnumber, city = addByAdmin.city, zipcode = addByAdmin.zipcode, name = addByAdmin.name, surname = addByAdmin.surname };
                var result = await _userManager.CreateAsync(user, addByAdmin.Password);
                var getsAdminRole = addByAdmin.role;
                if (email != null)
                {
                    ModelState.AddModelError(string.Empty, "Email already in use.");
                    return Page();
                }
                
                if (result.Succeeded && email == null)
                {
                    if (getsAdminRole == true)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
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
                    verifyMessage.To.Add(addByAdmin.Email);
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

                    return Page();
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        [Authorize(Roles = "Admin")]
        public void OnGet()
        {
        }
    }
}