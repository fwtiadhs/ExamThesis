using ExamThesis.Common;
using ExamThesis.Storage.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamThesis.Services.Services
{
    public interface ICreateQuestionService
    {
        Task Create(CreateQuestion question);
    }
    public class CreateQuestionService: ICreateQuestionService
    {
        private readonly ExamContext _db;
        public CreateQuestionService(ExamContext db)
        {
            _db = db;
        }
        public async Task Create(CreateQuestion question)
        {
                var model = new Question
                {
                    QuestionText = question.QuestionText,
                    Answers = question.Answers
                        .Where(answer => !string.IsNullOrEmpty(answer.Text))
                        .Select(answer => new Answer
                        {
                            Text = answer.Text,
                            IsCorrect = answer.IsCorrect

                        }).ToList(),
                    QuestionPoints = question.QuestionPoints,
                    NegativePoints = question.NegativePoints,
                    QuestionCategoryId = question.QuestionCategoryId
                };

                    _db.Questions.Add(model);
              await _db.SaveChangesAsync();
        }
    }
}
