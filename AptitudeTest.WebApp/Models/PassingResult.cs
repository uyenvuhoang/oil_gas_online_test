using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeTest.WebApp.Models
{
    public class PassingResult
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public int GeneralKnowledgeResult { get; set; }
        public int MathematicsResult { get; set; }
        public int ComputerTechnologyResult { get; set; }
    }
}
