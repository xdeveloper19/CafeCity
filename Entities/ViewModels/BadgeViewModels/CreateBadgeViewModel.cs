using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.BadgeViewModels
{
    public class CreateBadgeViewModel
    {
        [Display(Name ="Надпись")]
        public string Title { get; set; }

        [Display(Name ="Описание"),StringLength(maximumLength:50)]
        public string Description { get; set; }

        [Display(Name ="Изображение")]
        [Required]
        public IFormFile Imagine { get; set; }
    }
}
