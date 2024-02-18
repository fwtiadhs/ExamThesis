using ExamThesis.Common;
using ExamThesis.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamThesis.Services.Services
{
    public interface IExamCategoryService
    {
        Task AddCategoriesForExam(CreateExam exam, int examId);
    }
    public class ExamCategoryService : IExamCategoryService
    {
        private readonly ExamContext _db;
        public ExamCategoryService(ExamContext db)
        {
            _db = db;
        }
        public async Task AddCategoriesForExam(CreateExam exam, int examId)
        {
            var test = exam.SelectedCategories.ToList();
            foreach (var SelectedCategory in test)
            {
                var ExamCategory = new ExamCategory()
                {
                    ExamId = examId,
                    QuestionCategoryId = SelectedCategory.QuestionCategoryId
                    
                };
                _db.ExamCategories.Add(ExamCategory);
                

            }
            await _db.SaveChangesAsync();
        }
    }
}
