using InfSystemWebApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.CustomValidation
{
    public class IdExistValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            using (InfSystemContext db = new InfSystemContext())
            {
                return !(db.Employees.Find(value) is null);
            }
        }
    }
}