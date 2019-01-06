using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bytme.Models
{
    public class EditProductModel
    {
        public int id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Gender")]
        [RegularExpression(@"\b(male|female)\b", ErrorMessage = "Please choose either male or female.")]
        [StringLength(6, ErrorMessage = "Invalid input. Maximum is 6 characters.")]
        public string gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        [RegularExpression(@"^[a-zA-Z -]+$", ErrorMessage = "A description can only contain letters.")]
        [StringLength(700, ErrorMessage = "Invalid input. Maximum is 700 characters.")]
        public string description { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Long Description")]
        [RegularExpression(@"^[a-zA-Z -]+$", ErrorMessage = "A description can only contain letters.")]
        [StringLength(800, ErrorMessage = "Invalid input. Maximum is 800 characters.")]
        public string long_description { get; set; }

        [Required]
        [Display(Name = "Price")]
        [RegularExpression(@"(\d+\,\d{1,2})|\d{1,}", ErrorMessage = "Price invalid. Prices can only contain numbers and commas(max 999999,99).")]
        public float price { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Size")]
        [RegularExpression(@"^\b(XS|S|M|L|XL)\b?$", ErrorMessage = "Sizing must be in range of XS-XL")]
        public string size { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Photo URL")]
        public string photo_url { get; set; }

        [Required]
        [Display(Name = "Category Id")]
        [RegularExpression(@"^1[0-8]|[0-9]?$", ErrorMessage = "Catagory id should be a number between 1 and 18.")]
        public int category_id { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        [RegularExpression(@"^\d{1,6}?$", ErrorMessage = "quantity can only contain numbers of max 6 digits.")]
        public int quantity { get; set; }

        [Required]
        [Display(Name = "Sale")]
        [RegularExpression(@"^(0|1)?$", ErrorMessage = "sale can only be defined by 0 and 1.")]
        public int issales { get; set; }
    }
}
