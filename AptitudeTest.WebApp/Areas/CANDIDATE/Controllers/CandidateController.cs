using AptitudeTest.WebApp.Data;
using AptitudeTest.WebApp.Models;
using AptitudeTest.WebApp.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NuGet.Versioning;
using System.Runtime.InteropServices;

namespace AptitudeTest.WebApp.Areas.CANDIDATE.Controllers
{
    [Area("CANDIDATE")]
    public class CandidateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _emailConfig;

        public CandidateController(ApplicationDbContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }

        //Chau Anh edit lai Profile
        //Profile
        public IActionResult Profile()
        {
            var userBySession = HttpContext.Session.Get<User>("user");
            var user = _context.Users.FirstOrDefault(u => u.Id == userBySession.Id);
            if (user == null)
            {
                return NotFound();
            }

            var profileViewModel = new CandidateProfileViewModel(user);
            profileViewModel.FinalResultList = _context.FinalResults.Include(fr => fr.Exam).Where(fr => fr.UserId == user.Id).ToList();

            return View(profileViewModel);
        }

        [HttpPost]
        public IActionResult UpdateProfile(CandidateProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userToUpdate = _context.Users.FirstOrDefault(u => u.Id == model.Id);
                if (userToUpdate == null)
                {
                    return NotFound();
                }

                // Update the properties of the retrieved entity with the values from the model
                userToUpdate.UserName = model.UserName;
                userToUpdate.FirstName = model.FirstName;
                userToUpdate.LastName = model.LastName;
                userToUpdate.PhoneNumber = model.PhoneNumber;
                userToUpdate.BirthDay = model.BirthDay;


                // Save changes to the database
                _context.Users.Update(userToUpdate);
                _context.SaveChanges();

                TempData["Success"] = "Successfully Update Candidate";
                return RedirectToAction(nameof(Profile));
            }
            else
            {
                return View("Profile"); // Return to the EditProfile view with validation errors
            }
        }

        //Change Password
        public static string GenerateOTP(int length = 6)
        {
            const string chars = "0123456789";
            var random = new Random();
            var otp = new char[length];

            for (int i = 0; i < length; i++)
            {
                otp[i] = chars[random.Next(chars.Length)];
            }

            return new string(otp);
        }
        private void SendEmail(EmailMessage generateMessage)
        {
            var getMessage = CreateEmailMessage(generateMessage);
            Send(getMessage);
        }
        private MimeMessage CreateEmailMessage(EmailMessage generateMessage)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(" Webster@oilgascompany.com", _emailConfig.From));
            mimeMessage.To.AddRange(generateMessage.To);
            mimeMessage.Subject = generateMessage.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = generateMessage.Content };


            return mimeMessage;
        }
        private void Send(MimeMessage getMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.Username, _emailConfig.Password);
                client.Send(getMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        //Verify OTP to Change Password
        public IActionResult OTPVerify(int id)
        {
            var candidate = _context.Users.FirstOrDefault(u => u.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }

            string otp = GenerateOTP();
            HttpContext.Session.SetString("otp", otp);

            string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplate", "EmailOTP.html"); // Adjust the file path as per your actual location
            string emailContent;

            using (StreamReader reader = new StreamReader(templateFilePath))
            {
                emailContent = reader.ReadToEnd();
            }
            emailContent = emailContent.Replace("{OTP}", otp);
            var generateMessage = new EmailMessage(new string[] { candidate.Email }, "OTP Verification from Webster", emailContent);

            SendEmail(generateMessage);

            return View(candidate);
        }

        [HttpPost]
        public IActionResult OTPVerify(string verifyOtp)
        {
            if (string.IsNullOrEmpty(verifyOtp))
            {
                ModelState.AddModelError("OtpNullError", "Otp Verify must be fill");
                return View();
            }

            var getOTP = HttpContext.Session.GetString("otp");
            if (verifyOtp != getOTP)
            {
                ModelState.AddModelError("OtpWrongError", "Otp verification wrong, please try again");
                return View();
            }

            return RedirectToAction(nameof(ChangePassword));

        }

        //Change Password
        public IActionResult ChangePassword()
        {
            var userBySession = HttpContext.Session.Get<User>("user");
            var user = _context.Users.FirstOrDefault(u => u.Id == userBySession.Id);

            var vm = new ChangePasswordViewModel();
            vm.UserName = user.UserName;

            return View(vm);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel vm)
        {
            var userBySession = HttpContext.Session.Get<User>("user");
            var user = _context.Users.FirstOrDefault(u => u.Id == userBySession.Id);

            user.Password = vm.NewPasswordConfirm;

            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction(nameof(Profile));
        }


        //Index Page
        public IActionResult Index()
        {
            var userProfile = HttpContext.Session.Get<User>("user");
            if (userProfile == null)
            {
                return NotFound();
            }

            var exams = _context.Exams.ToList();

            var viewModel = new CandidateIndexViewModel
            {
                UserName = userProfile.UserName,
                Exams = exams,
                ExamCompletionStatus = new List<bool>(),
                CurrentExamIndex = -1
            };

            foreach (var exam in exams)
            {
                var hasTakenExam = _context.ExamResults.Any(er => er.ExamId == exam.Id && er.UserId == userProfile.Id);
                viewModel.ExamCompletionStatus.Add(hasTakenExam);

                if (!hasTakenExam && viewModel.CurrentExamIndex == -1)
                {
                    viewModel.CurrentExamIndex = exams.IndexOf(exam);
                }
            }

            return View(viewModel);
        }


        //Attend Exam
        public IActionResult AttendExam(int examId)
        {
            var model = new AttendExamViewModel();
            var userInfo = HttpContext.Session.Get<User>("user");
            var theLastExamId = _context.Exams.Max(e => e.Id);

            if (userInfo != null)
            {
                model.UserName = userInfo.UserName;
                model.CandidateId = userInfo.Id;
                model.QnAs = new List<QnAViewModel>();
                var getExam = _context.Exams.FirstOrDefault(e => e.Id == examId);
                model.ExamTitle = getExam.Title;
                model.ExamId = examId;
                model.ExamTimer = getExam.Time;

                if (_context.ExamResults.Any(er => er.ExamId == examId && er.UserId == userInfo.Id) == false)
                {
                    var qnaByExamList = _context.QnAs.Where(q => q.ExamId == examId).ToList();
                    var random = new Random();
                    var newRandomQnAList = new List<QnA>();

                    for (var i = 0; i < getExam.TakeRandomCount; i++)
                    {
                        var index = random.Next(0, qnaByExamList.Count);
                        newRandomQnAList.Add(qnaByExamList[index]);
                        qnaByExamList.RemoveAt(index);
                    }

                    model.QnAs = newRandomQnAList.Select(x => new QnAViewModel(x)).ToList();
                    model.Message = "";
                }
                else
                {
                    if (examId == theLastExamId)
                    {
                        model.NextExamId = 0;
                    }
                    else
                    {
                        var exams = _context.Exams.ToList();
                        for (var i = 0; i < exams.Count() - 1; i++)
                        {
                            if (exams[i].Id == examId)
                            {
                                model.NextExamId = exams[i + 1].Id;
                            }
                        }
                    }

                    model.Message = "You are already attend this Exam";
                }

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Authenticate", new { area = "" });
            }
        }

        [HttpPost]
        public IActionResult AttendExam(AttendExamViewModel vm)
        {
            foreach (var item in vm.QnAs)
            {

                ExamResult examResult = new ExamResult();
                examResult.UserId = vm.CandidateId;
                examResult.QnAId = item.Id;
                examResult.ExamId = item.ExamId;
                examResult.RecordAnswer = item.SelectedAnswer;
                _context.ExamResults.Add(examResult);
            }

            _context.SaveChanges();

            SaveToFinalResult(vm.CandidateId, vm.ExamId);

            var theLastExamId = _context.Exams.Max(e => e.Id);
            if (vm.ExamId == theLastExamId)
            {
                CheckIfPassing(vm.CandidateId);
                return RedirectToAction(nameof(ExitPage));
            }

            return RedirectToAction(nameof(Index));
        }

        private void CheckIfPassing(int candidateId)
        {
            if (_context.FinalResults.Where(fr => fr.UserId == candidateId).Any(fr => fr.Status == false))
            {
                return;
            }

            var allFinalResultByCandidate = _context.FinalResults.Include(fr => fr.Exam).Where(fr => fr.UserId == candidateId).ToList();
            var model = new PassingResult();
            model.UserId = candidateId;

            foreach (var item in allFinalResultByCandidate)
            {
                if (item.Exam.Title == "General Knowledge")
                {
                    model.GeneralKnowledgeResult = item.CountCorrect;
                }
                else if (item.Exam.Title == "Mathematics")
                {
                    model.MathematicsResult = item.CountCorrect;
                }
                else
                {
                    model.ComputerTechnologyResult = item.CountCorrect;
                }
            }

            _context.PassingResults.Add(model);
            _context.SaveChanges();
        }

        private void SaveToFinalResult(int candidateId, int examId)
        {
            var model = new FinalResult();
            model.UserId = candidateId;
            model.ExamId = examId;

            var getExamResults = _context.ExamResults.Where(er => er.UserId == candidateId && er.ExamId == examId).ToList();
            foreach (var item in getExamResults)
            {
                var getCorrectAnswer = _context.QnAs.FirstOrDefault(q => q.Id == item.QnAId);

                // Chau Anh
                if (item.RecordAnswer != null && getCorrectAnswer != null && item.RecordAnswer == getCorrectAnswer.TheAnswer)
                {
                    model.CountCorrect++;
                }
            }

            if (model.CountCorrect >= 3)
            {
                model.Status = true;
            }

            _context.FinalResults.Add(model);
            _context.SaveChanges();
        }


        //ExitPage
        public IActionResult ExitPage()
        {
            var userProfile = HttpContext.Session.Get<User>("user");
            if (userProfile == null)
            {
                return NotFound();
            }
            var viewModel = new CandidateIndexViewModel
            {
                UserName = userProfile.UserName,
            };

            return View(viewModel);
        }


        //Show Result 
        public async Task<IActionResult> ShowResult(int candidateId, int examId)
        {
            var model = new FinalResult();

            model.UserId = candidateId;
            model.ExamId = examId;

            var examResult = _context.ExamResults.Where(e => e.UserId == candidateId && e.ExamId == examId).ToList();

            foreach (var item in examResult)
            {
                var getCorrect = _context.QnAs.FirstOrDefault(q => q.Id == item.QnAId);

                if (item.RecordAnswer == getCorrect.TheAnswer)
                {
                    model.CountCorrect++;
                }
            }

            if (model.CountCorrect >= 3)
            {
                model.Status = true;
            }

            _context.FinalResults.Add(model);
            _context.SaveChanges();

            var vm = new FinalResultViewModel();
            vm.ExamInfo = _context.Exams.FirstOrDefault(e => e.Id == examId);
            vm.UserInfo = _context.Users.FirstOrDefault(u => u.Id == candidateId);
            vm.FinalResult = model;
            vm.NextExamId = examId + 1;

            return View(vm);
        }

    }
}
