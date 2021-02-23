using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookShopWebASP.Models
{
    public class ImageModel
    {
        /// <summary>  
        /// Gets or sets Image file.  
        /// </summary>  
        [Required]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase FileAttach { get; set; }

        public string Method { get; set; }

        /// <summary>  
        /// Gets or sets Image file list.  
        /// </summary>  
        public List<ImageData> Images { get; set; }
    }
}