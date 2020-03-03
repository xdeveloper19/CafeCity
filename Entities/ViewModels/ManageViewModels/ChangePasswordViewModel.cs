using System.ComponentModel.DataAnnotations;


namespace Entities.ViewModels.ManageViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Старый пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage ="The {0} must be at least {2} and at max {1} characters long.",MinimumLength =6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить новый пароль")]
        [Compare("NewPassword", ErrorMessage ="Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
