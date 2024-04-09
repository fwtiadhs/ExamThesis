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
        private ExamContext _db;
        public CategoryService(ExamContext db)
        {
            _db = db;
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
            var obj = await _db.QuestionCategories.FindAsync(id);
            _db.QuestionCategories.Remove(obj);
            await _db.SaveChangesAsync();
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

