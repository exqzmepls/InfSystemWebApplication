using InfSystemWebApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.CustomValidation
{
    public class IdRegistedValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int id = (int)value;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return !db.Users.Where(u => u.EmployeeId == id).Any();
            }
        }
    }
}