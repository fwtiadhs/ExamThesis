using ExamThesis.Storage.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamThesis.Services.Services
{
    public interface IExamService
    {
        Task<IEnumerable<Question>> GetExamQuestionsByExamId(int examId);
        Task DeleteByExamId(int examId);
    }

    public class ExamService : IExamService
    {
        private readonly ExamContext _db;
        public ExamService(ExamContext db)
        {
            _db = db;
        }

        public async Task DeleteByExamId(int examId)
        {
            var examsToDelete = await _db.Exams.FindAsync(examId);

            _db.Exams.Remove(examsToDelete);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Question>> GetExamQuestionsByExamId(int examId)
        {
            var exam = await _db.Exams.FirstAsync(q => q.ExamId == examId);
            var questions = _db.Questions.Where(q => q.QuestionCategoryId == exam.QuestionCategoryId).Include(q => q.Answers);
            return(questions); 
        }

        
    }
}
