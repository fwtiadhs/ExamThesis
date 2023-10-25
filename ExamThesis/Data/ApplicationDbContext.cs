using ExamThesis.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamThesis.Data
{
    public class ApplicationDbContext :DbContext
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options) 
        {
          
        }
        public DbSet<QuestionCategory> QuestionCategories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswer { get; set; }
        public DbSet<QuestionType> QuestionType { get; set; }
        public DbSet<AppFile> AppFile { get; set; }


    }


}
