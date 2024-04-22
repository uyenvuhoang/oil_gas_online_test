using AptitudeTest.WebApp.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AptitudeTest.WebApp.ViewModels
{
    public class QnAViewModel
    {
        public QnAViewModel()
        {

        }

        public QnAViewModel(QnA model)
        {
            Id = model.Id;
            ExamId = model.ExamId;
            Question = model.Question;
            Option1 = model.Option1;
            Option2 = model.Option2;
            Option3 = model.Option3;
            Option4 = model.Option4;
            TheAnswer = model.TheAnswer;
        }

        public QnA ConvertViewModel(QnAViewModel vm)
        {
            return new QnA()
            {
                Id = vm.Id,
                ExamId = vm.ExamId,
                Question = vm.Question,
                Option1 = vm.Option1,
                Option2 = vm.Option2,
                Option3 = vm.Option3,
                Option4 = vm.Option4,
                TheAnswer = vm.TheAnswer
            };
        }

        public int Id { get; set; }

        //1 QnA only belong to 1 Exam
        [ForeignKey("Exam")]
        public int ExamId { get; set; }

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

        public int SelectedAnswer { get; set; }
    }
}
