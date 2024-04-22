using System.ComponentModel.DataAnnotations;

namespace AptitudeTest.WebApp.ViewModels
{
    public class SendOTPViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "The OTP must be exactly 6 characters long.")]
        [Display(Name = "OTP")]
        public string OTP { get; set; }
    }
}
