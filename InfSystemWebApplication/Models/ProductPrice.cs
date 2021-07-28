using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class ProductPrice : IComparable
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Цена за единицу")]
        [Range(typeof(double), "0,01", "999999999")]
        public double ValuePerOneUnit { get; set; }

        [Display(Name = "Дата назначения цены")]
        [DataType(DataType.Date)]
        public DateTime SettingDate { get; set; }

        [Display(Name = "Товар")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public virtual ICollection<SoldProduct> SoldProducts { get; set; }

        [Display(Name = "Цена товара")]
        public string View => $"{ValuePerOneUnit} руб.";

        [Display(Name = "Товар")]
        public string ProductView => Product.View;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is ProductPrice price) return ValuePerOneUnit.CompareTo(price.ValuePerOneUnit);

            throw new ArgumentException("Object is not a ProductPrice");
        }

        public override string ToString() => View;
    }
}