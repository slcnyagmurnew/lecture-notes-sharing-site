using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moon.Entities
{
    public class Student: IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public string Department { get; set; }
    }
}
