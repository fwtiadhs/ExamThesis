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
        IEnumerable<Question> GetExamQuestionsByExamId(int examId);
    }

    public class ExamService : IExamService
    {
        private readonly ExamContext _db;
        public ExamService(ExamContext db)
        {
            _db = db;
        }

        public IEnumerable<Question> GetExamQuestionsByExamId(int id)
        {
            var exam = _db.Exams.Where(q => q.ExamId == id);
            var questions = _db.Questions.Where(q => q.QuestionCategoryId == exam.First().QuestionCategoryId).Include(q => q.Answers);
            return(questions); 
        }
    }
}
