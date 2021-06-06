using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class Sale
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Дата продажи")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Продавец")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Display(Name = "Покупатель")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [Display(Name = "Состав продажи")]
        public virtual List<SoldProduct> SoldProducts { get; set; }

        //public SoldProduct Contains(int priceId)
        //{
        //    return SoldProducts.Where(x => x.ProductPriceId == priceId)?.First();
        //}

        [Display(Name = "Итог")]
        public double? Total => SoldProducts?.Sum(x => x.ProductPrice.ValuePerOneUnit * x.Amount);

        [Display(Name = "Продажа")]
        public string View => $"Продажа {Id}";

        public string OnlyDate => Date.ToShortDateString();

        public override string ToString() => View;
    }
}