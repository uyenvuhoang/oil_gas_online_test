using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AptitudeTest.WebApp.Models
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Time { get; set; }

        [Required]
        public int TakeRandomCount { get; set; } = 5;

        //1 Exam have many QnA
        [ValidateNever]
        public List<QnA> QnAs { get; set; }
    }
}
