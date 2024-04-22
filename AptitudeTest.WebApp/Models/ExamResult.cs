using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeTest.WebApp.Models
{
    public class ExamResult
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Exam")]
        [Required]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        [ForeignKey("QnA")]
        [Required]
        public int QnAId { get; set; }
        public QnA QnA { get; set; }

        public int RecordAnswer { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
