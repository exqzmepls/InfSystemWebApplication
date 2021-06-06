using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class Product //: IComparable
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название товара")]
        [Display(Name = "Название")]
        [MaxLength(64)]
        public string Name { get; set; }

        [Display(Name = "Категория товара")]
        public int CategoryId { get; set; }

        [Display(Name = "Категория товара")]
        public virtual ProductCategory Category { get; set; }

        [Display(Name = "Поставщик товара")]
        public int SupplierId { get; set; }

        [Display(Name = "Поставщик товара")]
        public virtual Supplier Supplier { get; set; }

        [Display(Name = "Единица измерения")]
        public int UnitId { get; set; }

        [Display(Name = "Единица измерения")]
        public virtual Unit Unit { get; set; }

        public virtual List<ProductPrice> Prices { get; set; }

        [Display(Name = "Текущая цена")]
        public ProductPrice CurrentPrice => PriceOnDate(DateTime.Now.Date);

        public bool ContainsPriceOnDate(DateTime date)
        {
            return Prices.Where(x => x.SettingDate.Date == date.Date).Any();
        }

        public ProductPrice PriceOnDate(DateTime date)
        {
            List<ProductPrice> prices = Prices.Where(x => x.SettingDate <= date.Date).ToList();
            if (prices.Any())
            {
                DateTime settingDate = prices.Max(x => x.SettingDate);
                return prices.Where(x => x.SettingDate == settingDate).FirstOrDefault();
            }
            else return null;
        }

        //public int CompareTo(object obj)
        //{
        //    if (obj == null) return 1;

        //    if (obj is Product product) return Name.CompareTo(product.Name);

        //    throw new ArgumentException("Object is not a Product");
        //}

        [Display(Name = "Товар")]
        public string View => Name;

        public override string ToString() => View;
    }
}