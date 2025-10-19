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
    public interface ICategoryService
    {
        Task Create(ExamThesis.Common.QuestionCategory category);
        Task Edit(Common.QuestionCategory obj, int id);
        Task DeleteById(int id);
        
    }
    public class CategoryService : ICategoryService
    {
        private readonly ExamContext _db;
        private readonly IQuestionService _questionService;
        public CategoryService(ExamContext db, IQuestionService questionService)
        {
            _db = db;
            _questionService = questionService;
        }

        public async Task Create(ExamThesis.Common.QuestionCategory category)
        {
            var model = new Storage.Model.QuestionCategory()
            {
                QuestionCategoryName = category.QuestionCategoryName,
            };
            _db.QuestionCategories.Add(model);
            await _db.SaveChangesAsync();

        }

        public async Task DeleteById(int id)
        {
            // Delete all packages (and their questions/answers/joins) in this category first
            var packageIds = await _db.QuestionPackages
                .Where(p => p.QuestionCategoryId == id)
                .Select(p => p.PackageId)
                .ToListAsync();

            foreach (var pkgId in packageIds)
            {
                await _questionService.DeletePackage(pkgId);
            }

            // Remove exam-category mappings to avoid FK issues
            var examCats = await _db.ExamCategories
                .Where(ec => ec.QuestionCategoryId == id)
                .ToListAsync();
            if (examCats.Count > 0)
            {
                _db.ExamCategories.RemoveRange(examCats);
                await _db.SaveChangesAsync();
            }

            // Finally remove the category
            var obj = await _db.QuestionCategories.FindAsync(id);
            if (obj != null)
            {
                _db.QuestionCategories.Remove(obj);
                await _db.SaveChangesAsync();
            }
        }

        public async Task Edit(Common.QuestionCategory obj, int id)
        {
            var QCategoryFromDbFirst = _db.QuestionCategories
                .Where(u => u.QuestionCategoryId == id).FirstOrDefault();

            //QCategoryFromDbFirst.FileData = obj.FileData;
            QCategoryFromDbFirst.QuestionCategoryName = obj.QuestionCategoryName;

            _db.QuestionCategories.Update(QCategoryFromDbFirst);
            await _db.SaveChangesAsync();
        }

       
    }
}

