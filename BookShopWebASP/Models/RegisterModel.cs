using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookShopWebASP.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Your UserName is Required.")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Your Password is Required.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Compare("Password")]
        [Display(Name = "Confirm")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Your Full Name is Required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Your Email is Required.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Your Email is Invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Your Address is Required.")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Your Phone number is Required.")]
        [Display(Name = "Phone")]
        [Phone(ErrorMessage = "Your Phone number is Invalid.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Your Birthday is Required.")]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }
    }
}