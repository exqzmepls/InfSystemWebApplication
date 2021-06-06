using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class ProductClass
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<ProductSubclass> Subclasses { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProductSubclass
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int ClassId { get; set; }
        public virtual ProductClass Class { get; set; }

        public virtual ICollection<ProductGroup> Groups { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProductGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int SubclassId { get; set; }
        public virtual ProductSubclass Subclass { get; set; }

        public virtual ICollection<ProductSubgroup> Subgroups { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProductSubgroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int GroupId { get; set; }
        public virtual ProductGroup Group { get; set; }

        public virtual ICollection<ProductKind> Kinds { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProductKind
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int SubgroupId { get; set; }
        public virtual ProductSubgroup Subgroup { get; set; }

        public virtual ICollection<ProductCategory> Categories { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProductCategory //: IComparable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int KindId { get; set; }
        public virtual ProductKind Kind { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        //public int CompareTo(object other)
        //{
        //    if (other == null) return 1;

        //    if (other is ProductCategory category)
        //    {
        //        return Name.CompareTo(category.Name);
        //    }

        //    throw new ArgumentException("Object is not a ProductCategory");
        //}

        [Display(Name = "Категория товара")]
        public string View => Name;

        public override string ToString() => View;
    }
}