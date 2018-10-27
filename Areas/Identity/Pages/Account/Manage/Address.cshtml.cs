using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using bytme.Models;
using bytme.Data;

namespace bytme.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ILogger<AddressModel> _logger;

        public AddressModel(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            ILogger<AddressModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

       
        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            [StringLength(100, ErrorMessage = "Invalid input. Maximum is 100 characters.")]
            public string name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Surname")]
            [StringLength(100, ErrorMessage = "Invalid input. Maximum is 100 characters.")]
            public string surname { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Street")]
            [StringLength(48, ErrorMessage = "The longest street name in the Netherlands is 48 characters.")]
            public string street { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "House Number")]
            [StringLength(5, ErrorMessage = "The longest house number in the Netherlands is 5 characters.")]
            public string streetnumber { get; set; }
            

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "City")]
            [StringLength(28, ErrorMessage = "The longest place name in the Netherlands is 28 characters.")]
            public string city { get; set; }

            [Required]
            [DataType(DataType.PostalCode)]
            [Display(Name = "Postal Code")]
            [RegularExpression(@"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$", ErrorMessage = "Invalid zip, for example: 1234AB")]
            public string zipcode { get; set; }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // gets current user
            var user = await _userManager.GetUserAsync(User);

            // if no user is found, display error message
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // checks if current value is equal to new value
            // changes old value to input if false
            if (Input.name != user.name)
            {
                user.name = Input.name;
            }
            if (Input.surname != user.surname)
            {
                user.surname = Input.surname;
            }
            if (Input.street != user.street)
            {
                user.street = Input.street;
            }
            if (Input.streetnumber != user.streetnumber)
            {
                user.streetnumber = Input.streetnumber;
            }
            if (Input.city != user.city)
            {
                user.city = Input.city;
            }
            if (Input.zipcode != user.zipcode)
            {
                user.zipcode = Input.zipcode;
            }

            // update user
            var result = await _userManager.UpdateAsync(user);

            // dev logger message
            _logger.LogInformation("User added their address information successfully.");

            // user message
            StatusMessage = "Your address information has been updated.";

            return RedirectToPage();
        }
   
    }
}
