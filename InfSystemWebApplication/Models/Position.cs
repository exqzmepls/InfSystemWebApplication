using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class Position //: IComparable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        [Display(Name = "Должность")]
        public string View => Name;

        //public int CompareTo(object obj)
        //{
        //    if (obj == null) return 1;

        //    if (obj is Position position) return Name.CompareTo(position.Name);

        //    throw new ArgumentException("Object is not a Position");
        //}

        public override string ToString() => View;
    }

    public enum PositionId : int
    {
        ADMIN = 10, ASSISTANT = 20, SENIOR_ASSISTANT = 21
    }
}