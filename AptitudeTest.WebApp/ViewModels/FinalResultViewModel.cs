using AptitudeTest.WebApp.Models;

namespace AptitudeTest.WebApp.ViewModels
{
    public class FinalResultViewModel
    {
        public User UserInfo { get; set; }
        public Exam ExamInfo { get; set; }

        public int NextExamId { get; set; }

        public FinalResult FinalResult { get; set; }
    }
}
