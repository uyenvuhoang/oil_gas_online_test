using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeTest.WebApp.Models
{
    public class QnA
    {
        public int Id { get; set; }

        //1 QnA only belong to 1 Exam
        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        [ValidateNever]
        public Exam Exam { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Option1 { get; set; }
        [Required]
        public string Option2 { get; set; }
        [Required]
        public string Option3 { get; set; }
        [Required]
        public string Option4 { get; set; }

        [Required]
        [DisplayName("The Correct Answer")]
        public int TheAnswer { get; set; }
    }
}
