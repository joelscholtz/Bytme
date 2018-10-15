using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace bytme.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.PostalCode)]
            [Display(Name = "Postal Code")]
            public string zipcode { get; set; }
        }
        [BindProperty]
        public InputModel Country { get; set; }

        public class country
        {
            [Required]
            [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "You can only use letters.")]
            [Display(Name = "Country")]
            public string Country { get; set; }
        }
        [BindProperty]
        public InputModel City { get; set; }

        public class city
        {
            [Required]
            [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "You can only use letters.")]
            [Display(Name = "City")]
            public string City { get; set; }
        }
        [BindProperty]
        public InputModel Streetname { get; set; }

        public class street
        {
            [Required]
            [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "You can only use letters.")]
            [Display(Name = "Street name")]
            public string Streetname { get; set; }
        }
        [BindProperty]
        public InputModel Streetnumber { get; set; }

        public class streetnumber
        {
            [Required]
            [Display(Name = "Street number")]
            public string Streetnumber { get; set; }
        }
        

    }

}