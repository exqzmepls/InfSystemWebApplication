using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class Unit //: IComparable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Единица измерения")]
        public string Name { get; set; }

        public string ShortName { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        [Display(Name = "Единица измерения")]
        public string View => Name;

        //public int CompareTo(object obj)
        //{
        //    if (obj == null) return 1;

        //    if (obj is Unit unit) return Name.CompareTo(unit.Name);

        //    throw new ArgumentException("Object is not a Unit");
        //}

        public override string ToString() => View;
    }
}