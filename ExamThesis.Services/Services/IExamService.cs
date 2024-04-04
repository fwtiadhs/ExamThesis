
using ExamThesis.Common;
using ExamThesis.Storage;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace ExamThesis.Services.Services
{
    public interface IExamService
    {
        Task<IEnumerable<ExamQuestionViewModel>> GetExamQuestionsByExamId(int examId, string studentId);
        Task DeleteByExamId(int examId);
        Task CreateExam(CreateExam exam);
        Task<double> SubmitExam(int examId, List<int> selectedAnswers, string studentId, List<int> selectedQuestions);
       // Task <IEnumerable<ExamResult>> GetExamResultsById(int examId);


    }

    public class ExamService : IExamService
    {
        private readonly ExamContext _db;
        private readonly IExamCategoryService _categoryService;
        private readonly IMemoryCache _memoryCache;
        public ExamService(ExamContext db, IExamCategoryService categoryService, IMemoryCache memoryCache)
        {
            _db = db;
            _categoryService = categoryService;
            _memoryCache = memoryCache;
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
        public async Task<IEnumerable<ExamQuestionViewModel>> GetExamQuestionsByExamId(int examId, string studentId)
        {
            ExamQuestionViewModel examQuestionViewModel = null;
            var cacheKey = $"{studentId}_examId_questions";

            if (_memoryCache.TryGetValue<string>(cacheKey, out var examJson))
            {
               return JsonConvert.DeserializeObject<List<ExamQuestionViewModel>>(examJson);
            }
            else
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


                foreach (var category in examCategories)
                {
                    var questionPackages = await _db.QuestionPackages
                        .Where(qp => qp.QuestionCategoryId == category.QuestionCategoryId)
                        .Include(qp => qp.QuestionsInPackages)
                            .ThenInclude(qip => qip.Question)
                        .ToListAsync();

                    // Ομαδοποιούμε τα πακέτα ανά κατηγορία
                    // var groupedPackages = questionPackages.GroupBy(qp => qp.QuestionCategoryId);

                    // Επιλέγουμε τυχαίο πακέτο από κάθε ομάδα
                    var random = new Random().Next(0, questionPackages.Count() -1);
                    var selectedPackage = questionPackages[random];
                    
                        foreach (var questionInPackage in selectedPackage.QuestionsInPackages)
                        {
                            var question = questionInPackage.Question;
                            question.Answers = await _db.Answers
                           .Where(a => a.QuestionId == question.QuestionId)
                           .ToListAsync();

                            examQuestionViewModel = new ExamQuestionViewModel
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
                

                var json = JsonConvert.SerializeObject(examQuestionViewModels,Formatting.Indented,new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                if (!_memoryCache.TryGetValue<string>(cacheKey, out var x))
                {
                    //πως θα σεταρω το ποσο θα κραταει το model στην cache
                    _memoryCache.Set(cacheKey, json);
                }

                return examQuestionViewModels;
            }
        }


        public async Task<double> SubmitExam(int examId, List<int> selectedAnswers, string studentId, List<int> selectedQuestions)
        {

            var examQuestions = await _db.Questions.Where(x => selectedQuestions.Contains(x.QuestionId)).Include(x => x.Answers).ToListAsync();
            var exam = await _db.Exams.FindAsync(examId);

            if (exam == null || examQuestions == null)
            {
                // Επιστροφή κάποιας τιμής για ανεπιτυχή αίτηση
                return -1;
            }

            var earnedPoints = 0.0;

            foreach (var question in examQuestions)
            {
                var correctAnswer = question.Answers.Where(a => a.IsCorrect == true).First();
                Debug.WriteLine($"question: {question.QuestionText} ");

               
                Debug.WriteLine(correctAnswer);

                var userAnswers = selectedAnswers.ToList();

                // Υπολογισμός βαθμού μόνο για τις ερωτήσεις που έχουν επιλεγεί
                if (userAnswers.Contains(correctAnswer.Id))
                {
                    earnedPoints += question.QuestionPoints;
                }
                else
                {
                    earnedPoints -= question.NegativePoints;
                }
                Debug.WriteLine($"points: {earnedPoints} ");
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

