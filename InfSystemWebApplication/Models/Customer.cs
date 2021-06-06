using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfSystemWebApplication.Models
{
    public class Customer : Contact
    {
        [Display(Name = "Покупатель")]
        public string View => Name;

        public virtual List<Sale> Sales { get; set; }
    }
}