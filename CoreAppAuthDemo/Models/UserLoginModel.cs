using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage ="Email is mandatory.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is mandatory.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
