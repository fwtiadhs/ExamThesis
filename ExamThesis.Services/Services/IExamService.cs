using ExamThesis.Common;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
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
        Task CreateExam(CreateExam exam);
        Task<double> SubmitExam(int examId, List<int> selectedAnswers);

    }

    public class ExamService : IExamService
    {
        private readonly ExamContext _db;
        private readonly IExamCategoryService _categoryService;
        public ExamService(ExamContext db, IExamCategoryService categoryService)
        {
            _db = db;
            _categoryService = categoryService;
        }

        public async Task CreateExam(CreateExam exam)
        {
            var model = new Exam
            {
                ExamName = exam.ExamName,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,
                TotalPoints = exam.TotalPoints,

            };

            _db.Exams.Add(model);
            await _db.SaveChangesAsync();
            await _categoryService.AddCategoriesForExam(exam, model.ExamId);
        }

        public async Task DeleteByExamId(int examId)
        {
            var examsToDelete = await _db.Exams.FindAsync(examId);

            _db.Exams.Remove(examsToDelete);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Question>> GetExamQuestionsByExamId(int examId)
        {
            var exam = await _db.Exams.FindAsync(examId);
            var examCategories = _db.ExamCategories.Where(c => c.ExamId == examId).Select(x => x.QuestionCategoryId).ToList();
            var questions = _db.Questions.Where(q => examCategories.Contains(q.QuestionCategoryId)).Select(q => q).Include(q => q.Answers).ToList();
            //var examCategories =  _db.ExamCategories.Where(q => q.ExamId == examId).Select(q => q.QuestionCategoryId).ToList();
            // var questions = _db.Questions.Select(q => examCategories.Contains(q.QuestionCategoryId)).Include(q => q.Answers);
            return (questions);
        }

        public async Task<double> SubmitExam([FromBody] int examId, [FromBody] List<int> selectedAnswers)
        {
            var exam = await _db.Exams.FindAsync(examId);

            if (exam == null)
            {
                // Εάν δεν βρεθεί η εξέταση, εδώ μπορείτε να χειριστείτε την κατάσταση.
               throw new InvalidOperationException("Exam not found.");
            }

            var examCategories = _db.ExamCategories.Where(c => c.ExamId == examId).Select(x => x.QuestionCategoryId).ToList();
            var questions = _db.Questions.Where(q => examCategories.Contains(q.QuestionCategoryId)).Include(q => q.Answers).ToList();

            double earnedPoints = 0;

            foreach (var question in questions)
            {
                var correctAnswers = question.Answers.Where(a => a.IsCorrect == true).Select(a => a.Id);

                if (selectedAnswers.Intersect(correctAnswers).Count() == correctAnswers.Count())
                {
                    // Ο χρήστης έχει επιλέξει όλες τις σωστές απαντήσεις
                    earnedPoints += question.QuestionPoints;
                }
                else
                {
                    // Ο χρήστης έχει επιλέξει τουλάχιστον μία λανθασμένη απάντηση
                    earnedPoints -= question.NegativePoints;
                }
            }

            return earnedPoints;
        }
    }
}

