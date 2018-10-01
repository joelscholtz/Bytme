using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace bytme.Models
{
    public class ContactModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "You can only use letters.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Your name must be between 2 and 30 letters.")]
        public string name { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The subject must be between 2 and 50 characters.")]
        public string subject { get; set; }
        [Required]
        [StringLength(10000, MinimumLength = 2, ErrorMessage = "The message must be between 2 and 10'000 character.")]
        public string content { get; set; }
    }
}
