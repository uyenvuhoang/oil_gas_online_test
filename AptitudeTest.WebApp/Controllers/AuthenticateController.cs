using AptitudeTest.WebApp.Data;
using AptitudeTest.WebApp.Models;
using AptitudeTest.WebApp.Models.Enum;
using AptitudeTest.WebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using MimeKit;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Hosting;
using MimeKit.Cryptography;

namespace AptitudeTest.WebApp.Controllers
{
    public class AuthenticateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _emailConfig;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthenticateController(ApplicationDbContext context, EmailConfiguration emailConfig, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _emailConfig = emailConfig;
            _webHostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Chau Anh sau khi OTP match
        //Register
        public IActionResult Register(string email)
        {
            var model = new RegisterViewModel { 
                Email = email,
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.PhoneNumber.ToLower().Trim() == vm.PhoneNumber.ToLower().Trim()))
                {
                    ModelState.AddModelError("PhoneExistErr", "This Phone Number already Exist");
                    return View();
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == vm.Email);
                if (user == null)
                {
                    return NotFound();
                }
                string uniqueFileName = UploadedFile(vm);
                string uniqueResumeFileName = UploadedResumeFile(vm);
                user.FirstName = vm.FirstName;
                user.LastName = vm.LastName;
                user.PhoneNumber = vm.PhoneNumber;
                user.BirthDay = vm.BirthDay;
                user.Address = vm.Address;
                user.Gender = vm.Gender;
                user.CoverLetter = vm.CoverLetter;
                user.School = vm.School;
                user.ProfilePicture = uniqueFileName;
                user.ResumeFile = uniqueResumeFileName;
                _context.Users.Update(user);
                _context.SaveChanges();

                TempData["SuccessRegister"] = "Registration Success. Please wait to get Login Permision from MANAGER";

                return RedirectToAction(nameof(Login));
            }
            else
            {

                return View(vm);
            }
        }
        private string UploadedFile(RegisterViewModel vm)
        {
            string uniqueFileName = null;

            if (vm.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "profileimg");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + vm.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    vm.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        private string UploadedResumeFile(RegisterViewModel vm)
        {
            string uniqueFileName = null;

            if (vm.ResumeFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "resumes");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + vm.ResumeFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    vm.ResumeFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }


        //Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var getUser = _context.Users.Where(u => u.UserName == vm.UserName).FirstOrDefault();

                if (getUser == null)
                {
                    ModelState.AddModelError("NameErr", "Wrong UserName");
                    return View();
                }

                if (getUser.Password != vm.Password)
                {
                    ModelState.AddModelError("PassErr", "Wrong Password");
                    return View();
                }

                if (getUser.Role == (int)EnumRoles.MANAGER)
                {
                    HttpContext.Session.Set<User>("user", getUser);
                    return RedirectToAction("Dashboard", "Manager", new { area = "MANAGER" });
                }

                if (getUser.IsActive == false)
                {
                    TempData["Message"] = "Please wait to get Login Permision from MANAGER";
                    return RedirectToAction(nameof(Login));
                }

                HttpContext.Session.Set<User>("user", getUser);
                return RedirectToAction("Index", "Candidate", new { area = "CANDIDATE" });
            }
            else
            {
                return View(vm);
            }
        }


        //Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Set<User>("user", null);
            return RedirectToAction(nameof(Login));
        }



        // Generate OTP
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

        //Enter Email to Send OTP
        public IActionResult EnterEmail()
        {
            var model = new EmailViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendOTP(EmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EnterEmail", model);
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("EmailExist", "Email is already registered.");
                return View("EnterEmail");
            }

            // Generate the OTP
            string otp = GenerateOTP();

            var newUser = new User
            {
                Email = model.Email,
                OTP = otp
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();


            string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplate",  "EmailOTP.html"); // Adjust the file path as per your actual location
            string emailContent;

            using (StreamReader reader = new StreamReader(templateFilePath))
            {
                emailContent = reader.ReadToEnd();
            }
            emailContent = emailContent.Replace("{OTP}", otp);
            var generateMessage = new EmailMessage(new string[] { model.Email }, "OTP Verification from Webster", emailContent);
                
            SendEmail(generateMessage);

            // Redirect to the OTP verification view
            return RedirectToAction("VerifyOTP", "Authenticate", new { email = model.Email });
        }

        //
        public IActionResult VerifyOTP(string email)
        {
            var model = new SendOTPViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOTP(SendOTPViewModel model)
        {
            if (ModelState.IsValid)
            {
                var otpEntry = _context.Users.FirstOrDefault(e => e.Email == model.Email);
                if (otpEntry == null)
                {
                    // Email not found in the database, show error message
                    ModelState.AddModelError("OTP", "OTP verification failed. Please try again.");
                    return View();
                }

                if (model.OTP == otpEntry.OTP)
                {
                    // OTP is correct, update user status and redirect
                    _context.Users.Update(otpEntry);
                    _context.SaveChanges();

                    TempData["SuccessOTP"] = "OTP Verified Successfully";
                    return RedirectToAction(nameof(Register), new { email = model.Email });
                }
                else
                {
                    // Invalid OTP, show error message
                    ModelState.AddModelError("OTP", "Invalid OTP");
                    return View();
                }
            }
            return View(model);
        }

        //
        private void SendEmail(EmailMessage generateMessage)
        {
            var getMessage = CreateEmailMessage(generateMessage);
            Send(getMessage);
        }
        private MimeMessage CreateEmailMessage(EmailMessage generateMessage)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Webster@oilgascompany.com", _emailConfig.From));
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
    }
}
    


