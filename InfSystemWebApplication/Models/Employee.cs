using System;
using InfSystemWebApplication.CustomValidation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InfSystemWebApplication.Models
{
    //[EmployeeValidation(ErrorMessage = "Дата начала работы должна быть не меньше даты ухода с работы")]
    public class Employee //: IComparable
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Дата начала работы")]
        [DataType(DataType.Date)]
        public DateTime EntranceDate { get; set; }

        [Display(Name = "Дата окончания работы")]
        [DataType(DataType.Date)]
        public DateTime? LeaveDate { get; set; }

        [Display(Name = "Персональные данные сотрудника")]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        [Display(Name = "Должность")]
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        public virtual List<Sale> Sales { get; set; }

        [Display(Name = "Сотрудник")]
        public string View => $"{Position}: {Person}";

        public override string ToString() => View;

        //public int CompareTo(object obj)
        //{
        //    if (obj == null) return 1;

        //    if (obj is Employee employee)
        //    {
        //        int posCompare = Position.CompareTo(employee.Position);
        //        if (posCompare == 0)
        //        {
        //            return Person.CompareTo(employee.Person);
        //        }
        //        else return posCompare;
        //    }

        //    throw new ArgumentException("Object is not a Employee");
        //}
    }
}