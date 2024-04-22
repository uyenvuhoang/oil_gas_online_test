using AptitudeTest.WebApp.Data;
using AptitudeTest.WebApp.Models;
using AptitudeTest.WebApp.Models.Enum;
using AptitudeTest.WebApp.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Identity.Client;
using MimeKit;
using MimeKit.Utils;
using System.Net.Mime;

namespace AptitudeTest.WebApp.Areas.MANAGER.Controllers
{
    [Area("MANAGER")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _emailConfig;

        public ManagerController(ApplicationDbContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }
        
      
        ///////////
        //Dashboard
        public IActionResult Dashboard()
        {
            var sessionObj = HttpContext.Session.Get<User>("user");
            if (sessionObj == null || sessionObj.Role == (int)EnumRoles.CANDIDATE)
            {
                TempData["Message"] = "Only MANAGER can Access Dashboard, Please Login Again.";
                return RedirectToAction("Login", "Authenticate", new { area = "" });
            }

            DashboardViewModel vm = new DashboardViewModel();
            vm.CandidateCount = _context.Users.Count(u => u.Role == (int)EnumRoles.CANDIDATE);
            vm.ExamCount = _context.Exams.Count();
            vm.QuestionCount = _context.QnAs.Count();
            vm.PassingCount = _context.PassingResults.Count();

            return View(vm);
        }


        //////////////////////
        //Candidate Management
        public IActionResult AllCandidate()
        {
            var result = _context.Users.Where(u => u.Role == (int)EnumRoles.CANDIDATE).OrderBy(u => u.IsActive).ToList();
            return View(result);
        }

        //Detail Candidate
        public IActionResult DetailCandidate(int id)
        {
            var candidate = _context.Users.FirstOrDefault(u => u.Id == id);
            var resultOfCandidate = _context.FinalResults.Include(fr => fr.Exam).Where(fr => fr.UserId == id).ToList();

            var vm = new CandidateProfileViewModel(candidate);
            vm.FinalResultList = resultOfCandidate;

            return View(vm);
        }

        //Create Candidate
        public IActionResult CreateCandidate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCandidate(User model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim()))
                {
                    ModelState.AddModelError("EmailExistErr", "This Email already Exist");
                    return View();
                }

                if (_context.Users.Any(u => u.PhoneNumber.ToLower().Trim() == model.PhoneNumber.ToLower().Trim()))
                {
                    ModelState.AddModelError("PhoneExistErr", "This Phone Number already Exist");
                    return View();
                }


                _context.Users.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Successfully Create new Candidate";
                return RedirectToAction(nameof(AllCandidate));
            }
            else
            {
                return View(model);
            }
        }

        //Update Candidate
        public IActionResult UpdateCandidate(int id)
        {
            var result = _context.Users.FirstOrDefault(u => u.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        [HttpPost]
        public IActionResult UpdateCandidate(int id, User model)
        {
            if (ModelState.IsValid)
            {
                if(model.UserName == null)
                {
                    model.UserName = "";
                } 
                if(model.Password == null)
                {
                    model.Password = "";
                }
                if(model.ProfilePicture == null)
                {
                    model.ProfilePicture = "";
                }
                if(model.ResumeFile == null)
                {
                    model.ResumeFile = "";
                }

                _context.Users.Update(model);
                _context.SaveChanges();

                TempData["Success"] = "Successfully Update Candidate";
                return RedirectToAction(nameof(AllCandidate));
            }
            else
            {
                return View(model);
            }
        }


        //Delete Candidate
        public IActionResult DeleteCandidate(int id)
        {
            var candidate = _context.Users.FirstOrDefault(m => m.Id == id);
            var examResultByCandidate = _context.ExamResults.Where(er => er.UserId == id);
            var passingCandidate = _context.PassingResults.Where(pr => pr.UserId == id);
            var finalResultByCandidate = _context.FinalResults.Where(fr => fr.UserId == id);

            _context.FinalResults.RemoveRange(finalResultByCandidate);
            _context.PassingResults.RemoveRange(passingCandidate);
            _context.ExamResults.RemoveRange(examResultByCandidate);

            _context.Users.Remove(candidate);
            _context.SaveChanges();

            TempData["Success"] = "Successfully Delete Candidate";
            return RedirectToAction(nameof(AllCandidate));
        }

        //Verify Candidate
        public IActionResult VerifyCandidate(int id)
        {
            var candidate = _context.Users.FirstOrDefault(u => u.Id == id);
            if (candidate == null)
            {
                return NotFound(); ;
            }
            candidate.UserName = candidate.FirstName + candidate.LastName;
            candidate.Password = GenerateRandomPassword();
            candidate.IsActive = true;


            // <!-- Chau Anh
            string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplate", "EmailAdminCreateAccount.html"); // Adjust the file path as per your actual location
            string emailContent;
            using (StreamReader reader = new StreamReader(templateFilePath))
            {
                emailContent = reader.ReadToEnd();
            }
          
            emailContent = emailContent.Replace("{UserName}", candidate.UserName);
            emailContent = emailContent.Replace("{Password}", candidate.Password);
            

            var generateMessage = new EmailMessage(new string[] { candidate.Email }, "UserName and Password from Webster", emailContent);
            
            SendEmail(generateMessage);
            //  -->

            _context.Users.Update(candidate);
            _context.SaveChanges();

            TempData["Success"] = "Successfully Verify Candidate";
            return RedirectToAction(nameof(AllCandidate));
        }
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-"                        // non-alphanumeric
            };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
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


        /////////////////
        //Exam Management
        public IActionResult AllExam()
        {
            var result = _context.Exams.ToList();
            return View(result);
        }

        //Create Exam
        public IActionResult CreateExam()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateExam(Exam model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Exams.Any(e => e.Title.ToLower().Trim() == model.Title.ToLower().Trim()))
                {
                    ModelState.AddModelError("TitleExistErr", "This Exam Title already Exist");
                    return View();
                }

                _context.Exams.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Successfully Created new Exam";
                return RedirectToAction(nameof(AllExam));
            }
            else
            {
                return View(model);
            }
        }

        //Update Exam
        public IActionResult UpdateExam(int id)
        {
            var exam = _context.Exams.FirstOrDefault(e => e.Id == id);
            return View(exam);
        }

        [HttpPost]
        public IActionResult UpdateExam(int id, Exam model)
        {
            if (ModelState.IsValid)
            {
                _context.Exams.Update(model);
                _context.SaveChanges();

                TempData["Success"] = "Successfully Updated Exam";
                return RedirectToAction(nameof(AllExam));
            }
            else
            {
                return View(model);
            }
        }

        //Delete Exam
        public IActionResult DeleteExam(int id)
        {
            var exam = _context.Exams.Include(e => e.QnAs).FirstOrDefault(e => e.Id == id);
            var examResult = _context.ExamResults.Where(r => r.ExamId == exam.Id);
            var finalResult = _context.FinalResults.Where(f => f.ExamId == exam.Id);
            _context.ExamResults.RemoveRange(examResult);
            _context.FinalResults.RemoveRange(finalResult);

            // Delete the Exam
            _context.Exams.Remove(exam);
            _context.SaveChanges();

            TempData["Success"] = "Successfully Deleted Exam and its QnAs";
            return RedirectToAction(nameof(AllExam));
        }


        ////////////////
        //QnA Management
        public IActionResult AllQnA()
        {
            var allQnA = _context.QnAs.Include(q => q.Exam).OrderBy(q => q.ExamId).ToList();
            return View(allQnA);
        }

        [HttpPost]
        public IActionResult AllQnA(string findByExam = "" , string findByName = "")
        {
            if(string.IsNullOrEmpty(findByExam) && string.IsNullOrEmpty(findByName))
            {
                var allQnA = _context.QnAs.Include(q => q.Exam).OrderBy(q => q.ExamId).ToList();
                return View(allQnA);
            }

            if(string.IsNullOrEmpty(findByExam) && !string.IsNullOrEmpty(findByName))
            {
                var allQnAWithCondition = _context.QnAs.Include(q => q.Exam).Where(q => q.Question.ToLower().Trim().Contains(findByName.ToLower().Trim())).ToList();
                return View(allQnAWithCondition);
            }
            else if(!string.IsNullOrEmpty(findByExam) && string.IsNullOrEmpty(findByName))
            {
                var allQnAWithCondition = _context.QnAs.Include(q => q.Exam).Where(q => q.ExamId == Convert.ToInt32(findByExam)).ToList();
                return View(allQnAWithCondition);
            }
            else
            {
                var allQnAWithCondition = _context.QnAs.Include(q => q.Exam).Where(q => q.Question.ToLower().Trim().Contains(findByName.ToLower().Trim()) 
                                                                                     && q.ExamId == Convert.ToInt32(findByExam)).ToList();
                return View(allQnAWithCondition);
            }
        }

        public IActionResult AllQnAByExam(string examTitle)
        {
            var exam = _context.Exams.FirstOrDefault(e => e.Title == examTitle);
            var result = _context.QnAs.Include(q => q.Exam).Where(q => q.Exam.Title == examTitle).ToList();

            ViewBag.ExamTitle = examTitle;
            return View(result);
        }


        //CreateAllQnA
        public IActionResult CreateAllQnA()
        {
            var exams = _context.Exams.ToList();
            ViewBag.ExamList = new SelectList(exams, "Id", "Title");

            return View();
        }

        [HttpPost]
        public IActionResult CreateAllQnA(QnA model)
        {
            if (ModelState.IsValid)
            {
                if (_context.QnAs.Any(q => q.Question == model.Question) == true)
                {
                    ModelState.AddModelError("QExistErr", "This Question already Exist");
                    return View();
                }

                _context.QnAs.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Successfully Created new Question";
                return RedirectToAction(nameof(AllQnA));
            }
            else
            {
                var exams = _context.Exams.ToList();
                ViewBag.ExamList = exams.Select(e => new SelectListItem()
                {
                    Value = e.Id.ToString(),
                    Text = e.Title
                });
                return View(model);
            }
        }


        //UpdateAllQnA
        public IActionResult UpdateAllQnA(int id)
        {
            var qnaById = _context.QnAs.Include(q => q.Exam).FirstOrDefault(q => q.Id == id);
            var exams = _context.Exams.ToList();
            ViewBag.ExamList = new SelectList(exams, "Id", "Title");
            return View(qnaById);
        }

        [HttpPost]
        public IActionResult UpdateAllQnA(int id, QnA model)
        {
            if (ModelState.IsValid)
            {
                _context.QnAs.Update(model);
                _context.SaveChanges();

                TempData["Success"] = "Successfully Updated QnA";
                return RedirectToAction(nameof(AllQnA));
            }
            else
            {
                var exams = _context.Exams.ToList();
                ViewBag.ExamList = new SelectList(exams, "Id", "Title");
                return View(model);
            }
        }

        //DeleteQnA
        public IActionResult DeleteQnA(int id)
        {
            var qna = _context.QnAs.Include(q => q.Exam).FirstOrDefault(q => q.Id == id);
            var examResult = _context.ExamResults.Where(r => r.QnAId == qna.Id);
            if (qna == null)
            {
                return NotFound();
            }
            _context.ExamResults.RemoveRange(examResult);
            _context.QnAs.Remove(qna);
            _context.SaveChanges();

            TempData["Success"] = "Successfully Deleted QnA";
            return RedirectToAction(nameof(AllQnA));
        }


        //PassingResultDetail
        public IActionResult PassingResult()
        {
            var allPassingResults = _context.PassingResults.Include(pr => pr.User).ToList();
            return View(allPassingResults);
        }
    }
}
