namespace AptitudeTest.WebApp.ViewModels
{
    public class AttendExamViewModel
    {
        public string UserName { get; set; }
        public int CandidateId { get; set; }

        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int NextExamId { get; set; }
        public int ExamTimer { get; set; }

        public List<QnAViewModel> QnAs { get; set; }

        public string Message { get; set; }

    }
}
