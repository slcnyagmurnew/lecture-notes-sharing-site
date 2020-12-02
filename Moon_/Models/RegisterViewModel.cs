using System;
using System.ComponentModel.DataAnnotations;

namespace Moon_.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string surname { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string id { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string department { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirmPassword { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string email { get; set; }

    }
}
