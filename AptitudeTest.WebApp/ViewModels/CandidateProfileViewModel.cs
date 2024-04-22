using AptitudeTest.WebApp.Models;
using AptitudeTest.WebApp.Models.Enum;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AptitudeTest.WebApp.ViewModels
{
    public class CandidateProfileViewModel
    {
		public int Id { get; set; }
        public string UserName { get; set; }	
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDay { get; set; }
        public Gender Gender { get; set; }
        public School School { get; set; }
        public string Password { get; set; }

        [ValidateNever]
        public string ProfilePicture { get; set; }

        public List<FinalResult> FinalResultList { get; set; } = new List<FinalResult>();

        public CandidateProfileViewModel()
        {
        }

        public CandidateProfileViewModel(User user)
		{
            Id = user.Id;
			UserName = user.UserName;
			FirstName = user.FirstName;
			LastName = user.LastName;
            Email = user.Email;
			PhoneNumber = user.PhoneNumber;
			BirthDay = user.BirthDay;
            Gender = user.Gender;
            School = user.School;
            ProfilePicture = user.ProfilePicture;
			Password = user.Password;
		}

	}
}
