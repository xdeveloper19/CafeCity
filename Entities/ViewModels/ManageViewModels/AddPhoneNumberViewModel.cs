using System.ComponentModel.DataAnnotations;


namespace Entities.ViewModels.ManageViewModels
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
    }
}
