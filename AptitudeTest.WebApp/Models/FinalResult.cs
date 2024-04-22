using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeTest.WebApp.Models
{
    public class FinalResult
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }


        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public int CountCorrect { get; set; } = 0;

        public bool Status { get; set; } = false;

        public DateTime DateResultRecord { get; set; } = DateTime.Today;
    }
}
