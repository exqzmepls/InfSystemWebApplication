using InfSystemWebApplication.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class Person //: IComparable
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        [Display(Name = "Фамилия")]
        [MaxLength(64)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите отчество")]
        [Display(Name = "Отчество")]
        [MaxLength(64)]
        public string MiddleName { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Введите контактный телефон")]
        [Phone(ErrorMessage = "Введите корректный номер телефона")]
        [Display(Name = "Контактный телефон")]
        [MaxLength(16)]
        public string PhoneNumber { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        //public int CompareTo(object obj)
        //{
        //    if (obj == null) return 1;

        //    if (obj is Person person)
        //    {
        //        int surNameComp = Surname.CompareTo(person.Surname);
        //        if (surNameComp == 0)
        //        {
        //            int nameComp = Name.CompareTo(person.Name);
        //            if (nameComp == 0)
        //            {
        //                return MiddleName.CompareTo(person.MiddleName);
        //            }
        //            return nameComp;
        //        }
        //        return surNameComp;
        //    }
        //    else throw new ArgumentException("Object is not a Person");
        //}

        [Display(Name = "Персональные данные")]
        public string View => $"{Surname} {Name} {MiddleName} ({Id})";

        public override string ToString()
        {
            return View;
        }
    }
}