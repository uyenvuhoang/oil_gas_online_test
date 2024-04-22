using AptitudeTest.WebApp.Models.Enum;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeTest.WebApp.Models
{
    public class User
    {
        public int Id { get; set; }
        [ValidateNever]

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDay { get; set; } = DateTime.Today;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; } = Gender.Other;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string CoverLetter { get; set; } = string.Empty;

        /*[Required]
        public string Resume { get; set; } = string.Empty;*/

        [ValidateNever]
        public string ProfilePicture { get; set; } = string.Empty;

        [ValidateNever]
        public string ResumeFile { get; set; } = string.Empty;

        [Required]
        public School School { get; set; } = School.FPT_Aptech;


        //
        [ValidateNever]
        public string UserName { get; set; } = string.Empty;

        [ValidateNever]
        public string Password { get; set; } = string.Empty;

        public int Role { get; set; } = (int)EnumRoles.CANDIDATE;

        //
        public bool IsActive { get; set; } = false;

        //
        public string OTP { get; set; } = string.Empty;
    }
}
