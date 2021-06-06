using InfSystemWebApplication.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class SoldProduct
    {
        [Key]
        [Column(Order = 1)]
        public int SaleId { get; set; }
        public virtual Sale Sale { get; set; }

        [Key]
        [Column(Order = 2)]
        [Display(Name = "Товар")]
        public int ProductPriceId { get; set; }
        public virtual ProductPrice ProductPrice { get; set; }

        [Display(Name = "Количество")]
        [Range(typeof(double), "0,00001", "99999")]
        public double Amount { get; set; }

        //[Display(Name = "Цена")]
        //public double Price => ProductPrice.ValuePerOneUnit;

        [Display(Name = "Итог")]
        public double? Total => ProductPrice.ValuePerOneUnit * Amount;

        [Display(Name = "Товар")]
        public string ProductView => ProductPrice.Product.View;
    }
}