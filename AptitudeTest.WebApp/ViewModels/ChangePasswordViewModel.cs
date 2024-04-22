using System.ComponentModel.DataAnnotations;

namespace AptitudeTest.WebApp.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string UserName { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string NewPasswordConfirm { get; set; }
    }
}
