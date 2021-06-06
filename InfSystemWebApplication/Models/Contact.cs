using InfSystemWebApplication.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public abstract class Contact //: IComparable
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название организации")]
        [MaxLength(64)]
        [Display(Name = "Название организации")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите контактный телефон")]
        [MaxLength(16)]
        [Display(Name = "Контактный телефон")]
        [Phone(ErrorMessage = "Введите корректный номер телефона")]
        public string PhoneNumber { get; set; }

        //public override string ToString() => Name;

        //public int CompareTo(object obj)
        //{
        //    if (obj == null) return 1;

        //    if (obj is Contact contact) return Name.CompareTo(contact.Name);

        //    throw new ArgumentException("Object is not a Contact");
        //}
    }
}