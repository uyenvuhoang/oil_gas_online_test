using AptitudeTest.WebApp.Models;

namespace AptitudeTest.WebApp.ViewModels
{
    public class DashboardViewModel
    {
        public int CandidateCount { get; set; }
        public int ExamCount { get; set; }
        public int QuestionCount { get; set; }
        public int PassingCount { get; set; }
    }
}
