using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bytme.Models
{
    public class EditUserModel
    {
        public string id { get; set; }

        [EmailAddress]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Street")]
        [RegularExpression(@"^[a-zA-Z -.]+$", ErrorMessage = "A street name can only contain letters. If you want to add a house number, do it in the next field.")]
        [StringLength(48, ErrorMessage = "The longest street name in the Netherlands is 48 characters.")]
        public string street { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "House Number")]
        [RegularExpression(@"^[0-9]{1,5}([a-z]{1,2})?$", ErrorMessage = "A house number starts with at least 1 digit. House number addition is possible.")]
        [StringLength(7, ErrorMessage = "The longest house number in the Netherlands is 7 characters.")]
        public string streetnumber { get; set; }

        [DataType(DataType.PostalCode)]
        [Display(Name = "Postal Code")]
        [RegularExpression(@"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$", ErrorMessage = "Invalid zip, for example: 1234AB")]
        public string zipcode { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City")]
        [RegularExpression(@"^[a-zA-Z-]+$", ErrorMessage = "A city can only contain letters.")]
        [StringLength(28, ErrorMessage = "The longest place name in the Netherlands has 28 characters.")]
        public string city { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        [RegularExpression(@"^[a-zA-Z -]+$", ErrorMessage = "A name can only contain letters.")]
        [StringLength(100, ErrorMessage = "Invalid input. Maximum is 100 characters.")]
        public string name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Surname")]
        [RegularExpression(@"^[a-zA-Z -]+$", ErrorMessage = "A surname can only contain letters.")]
        [StringLength(100, ErrorMessage = "Invalid input. Maximum is 100 characters.")]
        public string surname { get; set; }

        public DateTime dt_created { get; set; }

        public bool role { get; set; }
    }
}
