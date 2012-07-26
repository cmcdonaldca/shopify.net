using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Security;

namespace SampleWebApp.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Shop name", Description = "Just the shop name part of shopname.myshopify.com")]
        public string ShopName { get; set; }
    }
}
