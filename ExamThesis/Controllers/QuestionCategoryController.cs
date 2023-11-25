using ExamThesis.Data;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamThesis.Controllers
{
    public class QuestionCategoryController : Controller
    {
        private readonly ExamContext _db;
        public QuestionCategoryController(ExamContext db)
        {
            _db= db;
        }
        public IActionResult Index()
        {
            IEnumerable<QuestionCategory> objQuestionCategoryList = _db.QuestionCategories.ToList();

            return View(objQuestionCategoryList) ;
        }
        //GET ACTION
        public IActionResult Create()
        {
            return View();
        }

        //POST ACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuestionCategory obj)
        {
            if (obj.QuestionCategoryName == obj.QuestionCategoryName.ToString()) {
                ModelState.AddModelError("CategoryName", "Cannot be the same name");
            }
            _db.QuestionCategories.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET ACTION
        public IActionResult Edit(int? id)
        {
            if(id==null|| id == 0)
            {
                return NotFound();
            }
            //var QCategoryFromDb = _db.QuestionCategories.Find(id);
            var QCategoryFromDbFirst = _db.QuestionCategories.FirstOrDefault(u => u.QuestionCategoryId == id);
           // var QCategoryFromDbSingle = _db.QuestionCategories.SingleOrDefault(u => u.CategoryId == id);
          
            if (QCategoryFromDbFirst == null)
            {
                return NotFound();
            }

            return View(QCategoryFromDbFirst);
        }

        //POST ACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(QuestionCategory obj)
        {
           // if (obj.CategoryName == obj.CategoryName.ToString())
            //{
              //  ModelState.AddModelError("CategoryName", "Cannot be the same name");
            //}

            if (ModelState.IsValid) { 

                _db.QuestionCategories.Update(obj);
                _db.SaveChanges();

             return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET ACTION
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var QCategoryFromDb = _db.QuestionCategories.Find(id);
            //var QCategoryFromDbFirst = _db.QuestionCategories.FirstOrDefault(u => u.CategoryId == id);
            // var QCategoryFromDbSingle = _db.QuestionCategories.SingleOrDefault(u => u.CategoryId == id);

            if (QCategoryFromDb == null)
            {
                return NotFound();
            }

            return View(QCategoryFromDb);
        }

        //POST ACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _db.QuestionCategories.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            _db.QuestionCategories.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
        }

    }
}
