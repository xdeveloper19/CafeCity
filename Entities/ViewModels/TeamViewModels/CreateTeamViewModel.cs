using System;
using System.ComponentModel.DataAnnotations;


namespace Entities.ViewModels.TeamViewModels
{
    public class CreateTeamViewModel
    {
        [Required,StringLength(30)]
        [Display(Name="Наименование")]
        public string Name { get; set; }
        
        [StringLength(1000)]
        [Display(Name="Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Город")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата основания")]
        public DateTime Date { get; set; }
        
        [DataType(DataType.Url)]
        [Display(Name = "Ссылка на группу ВКонтакте")]
        public string Url { get; set; }

        [Required]
        [Display(Name = "Направление")]
        public string Section { get; set; }
    }
}
