using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookShopWebASP.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "You must enter password")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must enter new password")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        [Display(Name = "Confirm")]
        public string ConfirmPassword { get; set; }
    }
}