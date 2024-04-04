
using ExamThesis.Common;
using ExamThesis.Storage;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace ExamThesis.Services.Services
{
    public interface IExamService
    {
        Task<IEnumerable<ExamQuestionViewModel>> GetExamQuestionsByExamId(int examId);
        Task DeleteByExamId(int examId);
        Task CreateExam(CreateExam exam);
        Task<double> SubmitExam(int examId, List<int> selectedAnswers, string studentId);
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
        public async Task<IEnumerable<ExamQuestionViewModel>> GetExamQuestionsByExamId(int examId)
        {
            var exam = await _db.Exams.FindAsync(examId);

            if (exam == null)
            {
                return Enumerable.Empty<ExamQuestionViewModel>();
            }

            var examQuestionViewModels = new List<ExamQuestionViewModel>();
            var examCategories = await _db.ExamCategories
                .Where(ec => ec.ExamId == examId)
                .ToListAsync();

            var random = new Random();

            foreach (var category in examCategories)
            {
                var questionPackages = await _db.QuestionPackages
                    .Where(qp => qp.QuestionCategoryId == category.QuestionCategoryId)
                    .Include(qp => qp.QuestionsInPackages)
                        .ThenInclude(qip => qip.Question)
                    .ToListAsync();

                // Ομαδοποιούμε τα πακέτα ανά κατηγορία
                var groupedPackages = questionPackages.GroupBy(qp => qp.QuestionCategoryId);

                // Επιλέγουμε τυχαίο πακέτο από κάθε ομάδα
                var selectedPackages = groupedPackages.Select(group => group.OrderBy(qp => random.Next()).First());

                foreach (var questionPackage in selectedPackages)
                {
                    foreach (var questionInPackage in questionPackage.QuestionsInPackages)
                    {
                        var question = questionInPackage.Question;
                         question.Answers = await _db.Answers
                        .Where(a => a.QuestionId == question.QuestionId)
                        .ToListAsync();

                        var examQuestionViewModel = new ExamQuestionViewModel
                        {
                            StartTime = exam.StartTime,
                            EndTime = exam.EndTime,
                            TotalPoints = exam.TotalPoints,
                            ExamId = exam.ExamId,
                            ExamName = exam.ExamName,
                            PassGrade = (double)exam.PassGrade,
                            QuestionId = question.QuestionId,
                            QuestionCategoryId = question.QuestionCategoryId,
                            NegativePoints = question.NegativePoints,
                            QuestionText = question.QuestionText,
                            QuestionPoints = question.QuestionPoints,
                            PackageId = question.PackageId,
                            Answers = question.Answers,
                            QuestionsInPackages = question.QuestionsInPackages
                        };

                        examQuestionViewModels.Add(examQuestionViewModel); 
                    }
                }
            }

            return examQuestionViewModels;
        }

        public async Task<IEnumerable<ExamResult>> GetExamResultsById(int examId)
        {
           var result =  _db.ExamResults.Where(e => e.ExamId == examId).ToList();
            return (result);
        }



        public async Task<double> SubmitExam(int examId, List<int> selectedAnswers, string studentId)
        {

            var examQuestions = await GetExamQuestionsByExamId(examId);
            var exam = await _db.Exams.FindAsync(examId);

            if (exam == null || examQuestions == null)
            {
                // Επιστροφή κάποιας τιμής για ανεπιτυχή αίτηση
                return -1;
            }

            var earnedPoints = 0.0;

            foreach (var question in examQuestions)
            {
                var correctAnswers = question.Answers.Where(a => a.IsCorrect == true).Select(a => a.Id);
                var userAnswers = selectedAnswers.Where(sa => sa == question.QuestionId);
                
                    // Υπολογισμός βαθμού μόνο για τις ερωτήσεις που έχουν επιλεγεί
                    if (correctAnswers.SequenceEqual(userAnswers))
                    {
                        earnedPoints += question.QuestionPoints;
                    }
                    else
                    {
                        earnedPoints -= question.NegativePoints;
                    }
                }

                var examResult = new ExamResult
                {
                    StudentId = studentId,
                    ExamId = examId,
                    Grade = earnedPoints
                };

                _db.ExamResults.Add(examResult);
                await _db.SaveChangesAsync();

                return earnedPoints;
            
        }
    }
}

