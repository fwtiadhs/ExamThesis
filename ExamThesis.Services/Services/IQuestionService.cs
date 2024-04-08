using Azure.Core;
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
    public interface IQuestionService
    {
        Task Create(CreateQuestion question);
        Task DeleteById(int id);
        Task<bool> DeleteAnswer(string answerText);
        Task CreatePackage(QuestionPackage package);
        Task DeletePackage(int packageId);
    }
    public class QuestionService : IQuestionService
    {
        private readonly ExamContext _db;
        public QuestionService(ExamContext db)
        {
            _db = db;
        }
        public async Task Create(CreateQuestion question)
        {
            var model = new ExamThesis.Storage.Model.Question
            {
                QuestionText = question.QuestionText,
                Answers = question.Answers
                    .Where(answer => !string.IsNullOrEmpty(answer.Text))
                    .Select(answer => new ExamThesis.Storage.Model.Answer
                    {
                        Text = answer.Text,
                        IsCorrect = answer.IsCorrect

                    }).ToList(),
                QuestionPoints = question.QuestionPoints,
                NegativePoints = question.NegativePoints,
                QuestionCategoryId = question.QuestionCategoryId,
                PackageId = question.PackageId
            };

            _db.Questions.Add(model);
            await _db.SaveChangesAsync();

            // Προσθήκη ερώτησης στο QuestionInPackage
            var questionInPackage = new QuestionsInPackage
            {
                PackageId = model.PackageId,
                QuestionId = model.QuestionId
            };

            _db.QuestionsInPackages.Add(questionInPackage);
            await _db.SaveChangesAsync();
        }

        public async Task CreatePackage(QuestionPackage package)
        {
            var model = new QuestionPackage
            {
                PackageName = package.PackageName,
                QuestionCategoryId = package.QuestionCategoryId,
                FileData = package.FileData
            };

            _db.QuestionPackages.Add(model);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAnswer(string answerText)
        {
            try
            {
                var answer = await _db.Answers.FirstOrDefaultAsync(a => a.Text == answerText);

                if (answer == null)
                    return false; // Η απάντηση δε βρέθηκε

                _db.Answers.Remove(answer);
                await _db.SaveChangesAsync();
                return true; // Η απάντηση διαγράφηκε με επιτυχία
            }
            catch
            {
                return false; // Σφάλμα κατά τη διαγραφή
            }
        }

        public async Task DeleteById(int id)
        {
            var questionToDelete = _db.Questions.Find(id);
            var answersToDelete = _db.Answers.Where(a => a.QuestionId == id);

            // Προσθέστε κώδικα για τη διαγραφή των εγγραφών από τον πίνακα QuestionsInPackage
            var questionsInPackageToDelete = _db.QuestionsInPackages.Where(qip => qip.QuestionId == id);
            _db.QuestionsInPackages.RemoveRange(questionsInPackageToDelete);

            _db.Answers.RemoveRange(answersToDelete);

            _db.Questions.Remove(questionToDelete);
           await _db.SaveChangesAsync();
            
        }

        public async Task DeletePackage(int packageId)
        {
            var package = await _db.QuestionPackages.FindAsync(packageId);

            if (package != null)
            {
                _db.QuestionPackages.Remove(package);
                await _db.SaveChangesAsync();
            }
        }
    }
}

    
