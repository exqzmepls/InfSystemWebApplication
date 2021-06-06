using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class Supplier : Contact
    {
        [Display(Name = "Поставщик")]
        public string View => Name;

        [Display(Name = "Товары")]
        public virtual List<Product> Products { get; set; }
    }
}