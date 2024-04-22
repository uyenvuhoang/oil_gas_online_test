using AptitudeTest.WebApp.Models;

namespace AptitudeTest.WebApp.ViewModels
{
    public class CandidateIndexViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<Exam> Exams { get; set; }
        public int CurrentExamIndex { get; set; }
        public List<bool> ExamCompletionStatus { get; set; }
        // Add any other properties relevant to the view if needed
    }

}

 