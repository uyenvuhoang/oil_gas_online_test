using AptitudeTest.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AptitudeTest.WebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<QnA> QnAs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FinalResult> FinalResults { get; set; }
        public DbSet<PassingResult> PassingResults { get; set; }
    }
}
