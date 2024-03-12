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
        Task <IEnumerable<ExamResult>> GetExamResultsById(int examId);

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
                PassGrade = exam.PassGrade,

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
            var random = new Random();
            var exam = await _db.Exams.FindAsync(examId);
            var examCategories = _db.ExamCategories.Where(c => c.ExamId == examId).Select(x => x.QuestionCategoryId).ToList();

            var questions = new List<Question>();

            foreach (var category in examCategories)
            {
                var availablePackages = await _db.QuestionPackages
                    .Where(qp => qp.QuestionCategoryId == category)
                    .OrderBy(qp => Guid.NewGuid())
                    .Take(1)
                    .ToListAsync();

                if (availablePackages.Any())
                {
                    var selectedPackage = availablePackages[0];

                    var questionsInPackage = await _db.QuestionsInPackages
                        .Where(qip => qip.PackageId == selectedPackage.PackageId)
                        .Select(qip => qip.Question)
                        .ToListAsync();

                    var shuffledQuestions = questionsInPackage.OrderBy(q => random.Next()).ToList();

                    // Χρησιμοποιήστε Include για να φορτώσετε τις σχέσεις
                    questions.AddRange(shuffledQuestions.Select(q => _db.Questions.Include(q => q.Answers).FirstOrDefault(x => x.QuestionId == q.QuestionId)));
                }
            }





            //var questions = _db.Questions.Where(q => examCategories.Contains(q.QuestionCategoryId)).Select(q => q).Include(q => q.Answers).ToList();
            //var examCategories =  _db.ExamCategories.Where(q => q.ExamId == examId).Select(q => q.QuestionCategoryId).ToList();
            // var questions = _db.Questions.Select(q => examCategories.Contains(q.QuestionCategoryId)).Include(q => q.Answers);
            return (questions);
        }

        public async Task<IEnumerable<ExamResult>> GetExamResultsById(int examId)
        {
           var result =  _db.ExamResults.Where(e => e.ExamId == examId).ToList();
            return (result);
        }

       

        public async Task<double> SubmitExam([FromBody] int examId, [FromBody] List<int> selectedAnswers)
        {
            var exam = await _db.Exams.FindAsync(examId);

            if (exam == null)
            {
               throw new InvalidOperationException("Exam not found.");
            }

            var examCategories = _db.ExamCategories.Where(c => c.ExamId == examId).Select(x => x.QuestionCategoryId).ToList();
            var questions = _db.Questions
                .Where(q => examCategories.Contains(q.QuestionCategoryId))
                .Include(q => q.Answers)
                .ToList();

            double earnedPoints = 0;

            foreach (var question in questions)
            {
                var correctAnswers = question.Answers.Where(a => a.IsCorrect == true).Select(a => a.Id);
                var userSelectedCorrect = selectedAnswers.Intersect(correctAnswers).Count() == correctAnswers.Count();
                var userSelectedIncorrect = selectedAnswers.Except(correctAnswers).Any();

                if (userSelectedCorrect)
                {
                    // Ο χρήστης έχει επιλέξει όλες τις σωστές απαντήσεις
                    earnedPoints += question.QuestionPoints;
                }
                else if (userSelectedIncorrect)
                {
                    // Ο χρήστης έχει επιλέξει τουλάχιστον μία λανθασμένη απάντηση
                    earnedPoints -= question.QuestionPoints - Math.Max(0, question.NegativePoints);
                }
            }

            var examResult = new ExamResult
            {
                ExamId = examId,
                Grade = earnedPoints
            };

            _db.ExamResults.Add(examResult);
            await _db.SaveChangesAsync();

            return earnedPoints;
        }
    }
}

