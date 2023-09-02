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
    }
}
