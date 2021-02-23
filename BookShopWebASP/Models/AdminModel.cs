using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopWebASP.Models
{
    public class AdminModel
    {
        public string Method { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int? Point { get; set; }
        public string IdUserAuthority { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? DateInput { get; set; }
        public DateTime? DateOutput { get; set; }
        public DateTime? DateCheckOut { get; set; }
        public int? IsCheckOut { get; set; }
        public int? Count { get; set; }
        public double? InputPrice { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public string IdCategory { get; set; }
        public string IdUnit { get; set; }
        public string IdImageData { get; set; }
        public string IdInput { get; set; }
        public string IdOutput { get; set; }
        public string IdSupplier { get; set; }
        public string IdCustomer { get; set; }
        public string IdProduct { get; set; }
        public string IdBasket { get; set; }
    }
}