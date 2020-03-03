using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.ManageViewModels
{
    public class AddMiddleNameViewModel
    {
        [Required]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }
    }
}
