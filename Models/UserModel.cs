using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace bytme.Models
{
    public class UserModel : IdentityUser
    {
        public override string Id { get; set; }
        public override string Email { get; set; }
        public override string UserName { get; set; }
        public override string PasswordHash { get; set; }
        public string zipcode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string streetnumber { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
    }
}
