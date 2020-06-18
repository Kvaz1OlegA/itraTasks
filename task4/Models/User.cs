using Microsoft.AspNetCore.Identity;
using System;

namespace task4.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public DateTime registrationDate { get; set; }
        public DateTime lastLoginDate { get; set; }
        public bool Blocked { get; set; }
    }
}