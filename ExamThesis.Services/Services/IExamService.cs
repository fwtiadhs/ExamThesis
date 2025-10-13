using ExamThesis.Common;
using ExamThesis.Storage;
using ExamThesis.Storage.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExamThesis.Services.Services
{
    public interface IExamService
    {
        Task<IEnumerable<ExamQuestionViewModel>> GetExamQuestionsByExamId(int examId, string studentId);
        Task DeleteByExamId(int examId);
        Task CreateExam(CreateExam exam);
        Task<double> SubmitExam(int examId, List<int> selectedAnswers, string studentId, List<int> selectedQuestions);
    }

    public class ExamService : IExamService
    {
        private readonly ExamContext _db;
        private readonly IExamCategoryService _categoryService;
        private readonly IMemoryCache _memoryCache;

        private static readonly ThreadLocal<Random> _rng = new(() => new Random());

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
                ShowGrade = exam.ShowGrade,
            };

            _db.Exams.Add(model);
            await _db.SaveChangesAsync();
            await _categoryService.AddCategoriesForExam(exam, model.ExamId);
        }

        public async Task DeleteByExamId(int examId)
        {
            var examToDelete = await _db.Exams.FindAsync(examId);
            if (examToDelete != null)
            {
                var examResultsToDelete = await _db.ExamResults.Where(er => er.ExamId == examId).ToListAsync();
                _db.ExamResults.RemoveRange(examResultsToDelete);

                var examCategoriesToDelete = await _db.ExamCategories.Where(ec => ec.ExamId == examId).ToListAsync();
                _db.ExamCategories.RemoveRange(examCategoriesToDelete);

                _db.Exams.Remove(examToDelete);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ExamQuestionViewModel>> GetExamQuestionsByExamId(int examId, string studentId)
        {
            var cacheKey = $"{studentId}_{examId}_questions_v2";

            if (_memoryCache.TryGetValue<string>(cacheKey, out var examJson))
            {
                return JsonConvert.DeserializeObject<List<ExamQuestionViewModel>>(examJson)
                       ?? Enumerable.Empty<ExamQuestionViewModel>();
            }

            var exam = await _db.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.ExamId == examId);
            if (exam == null)
                return Enumerable.Empty<ExamQuestionViewModel>();

            var result = new List<ExamQuestionViewModel>();

            var categoryIds = await _db.ExamCategories
                .AsNoTracking()
                .Where(ec => ec.ExamId == examId)
                .Select(ec => ec.QuestionCategoryId)
                .ToListAsync();

            foreach (var categoryId in categoryIds)
            {
                var questionPackages = _db.QuestionPackages
                    .AsNoTracking()
                    .Where(qp => qp.QuestionCategoryId == categoryId)
                    .Include(qp => qp.QuestionsInPackages)
                        .ThenInclude(qip => qip.Question)
                    .ToList();

                if (questionPackages.Count == 0)
                    continue;

                var viablePackages = questionPackages
                    .Where(p => p != null && p.QuestionsInPackages != null && p.QuestionsInPackages.Any(qip => qip != null && qip.Question != null))
                    .ToList();

                if (viablePackages.Count == 0)
                    continue;

                // Select exactly one random package for this category
                var selectedPackage = viablePackages[_rng.Value!.Next(viablePackages.Count)];
                if (selectedPackage?.QuestionsInPackages == null)
                    continue;

                foreach (var qip in selectedPackage.QuestionsInPackages.Where(x => x != null && x.Question != null))
                {
                    var question = qip!.Question!;

                    var answers = await _db.Answers
                        .AsNoTracking()
                        .Where(a => a.QuestionId == question.QuestionId)
                        .ToListAsync();

                    result.Add(new ExamQuestionViewModel
                    {
                        StartTime = exam.StartTime,
                        EndTime = exam.EndTime,
                        TotalPoints = exam.TotalPoints,
                        ExamId = exam.ExamId,
                        ExamName = exam.ExamName ?? string.Empty,
                        PassGrade = exam.PassGrade ?? 0,
                        ShowGrade = exam.ShowGrade,
                        QuestionId = question.QuestionId,
                        QuestionCategoryId = question.QuestionCategoryId,
                        NegativePoints = question.NegativePoints,
                        QuestionText = question.QuestionText,
                        QuestionPoints = question.QuestionPoints,
                        PackageId = selectedPackage.PackageId,
                        Answers = answers,
                        QuestionsInPackages = question.QuestionsInPackages,
                        FileData = selectedPackage.FileData,
                        PackageName = selectedPackage.PackageName,
                        FileType = selectedPackage.FileType,
                        ImageData = question.ImageData,
                        ImageType = question.ImageType
                    });
                }
            }

            var json = JsonConvert.SerializeObject(
                result,
                Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(120)
            };
            _memoryCache.Set(cacheKey, json, cacheOptions);

            return result;
        }

        public async Task<double> SubmitExam(int examId, List<int> selectedAnswers, string studentId, List<int> selectedQuestions)
        {
            var examQuestions = await _db.Questions
                .Where(x => selectedQuestions.Contains(x.QuestionId))
                .Include(x => x.Answers)
                .ToListAsync();

            var exam = await _db.Exams.FindAsync(examId);
            if (exam == null || examQuestions == null)
                return -1;

            double earnedPoints = 0.0;

            foreach (var question in examQuestions)
            {
                if (question.Answers == null || question.Answers.Count == 0)
                    continue;

                var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect == true);
                if (correctAnswer == null)
                    continue;

                if (selectedAnswers.Contains(correctAnswer.Id))
                    earnedPoints += question.QuestionPoints;
                else
                    earnedPoints -= question.NegativePoints;
            }

            if (earnedPoints < 0) earnedPoints = 0.0;

            var examResult = new ExamResult
            {
                StudentId = studentId,
                ExamId = examId,
                Grade = earnedPoints
            };

            _db.ExamResults.Add(examResult);
            await _db.SaveChangesAsync();

            _memoryCache.Remove($"{studentId}_{examId}_questions_v2");

            return earnedPoints;
        }
    }
}

