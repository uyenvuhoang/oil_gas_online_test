using AptitudeTest.WebApp.Models;
using AptitudeTest.WebApp.Models.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace AptitudeTest.WebApp.ViewModels
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime BirthDay { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public string CoverLetter { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile ResumeFile { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile ProfileImage { get; set; }

        public School School { get; set; }
    }
}
