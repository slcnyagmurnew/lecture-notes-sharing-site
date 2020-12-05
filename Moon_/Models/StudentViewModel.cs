using Moon.Entities;
using System.ComponentModel.DataAnnotations;

namespace Moon
{
    public class StudentViewModel
    {
        [Display(Name = "User name"), Required(ErrorMessage = "Please enter your username.")]
        public string id { get; set; }

        [StringLength(100, ErrorMessage = "{0} must at least {2} length", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password"), Required(ErrorMessage = "Please enter your password.")]
        public string password { get; set; }
    }
}