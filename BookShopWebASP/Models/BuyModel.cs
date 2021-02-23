using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookShopWebASP.Models
{
    public class BuyModel
    {
        public string FullName { get; set; }
        public string ProductID { get; set; }
        public string ProductDisplayName { get; set; }
        public double ProductPrice { get; set; }
        public int Count { get; set; }

        [Required(ErrorMessage="You must enter password to buy.")]
        public string Password { get; set; }
        public bool FromBasket { get; set; }
    }
}