using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookShopWebASP.Models
{
    public class ManageModel
    {

        [Required(ErrorMessage = "You must enter password")]
        [Display(Name = "Password")]
        public string Password { get; set; }

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
        [DataType(DataType.Date)]
        public string Birthday { get; set; }
    }
}