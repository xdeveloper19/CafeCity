using System;
using System.ComponentModel.DataAnnotations;


namespace Entities.ViewModels.AccountViewModels
{
    public class UserProfileViewModel:BaseResponseObject
    {
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Display(Name = "Группа")]
        public string Team { get; set; }

        [Display(Name = "Факультет")]
        public string Section { get; set; }

        [Display(Name = "Должность")]
        public string Rang { get; set; }

        public byte[] Imagines { get; set; }

        public Guid TeamId { get; set; }

    }
}
