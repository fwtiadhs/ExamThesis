using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamThesis.Controllers
{
    public class QuestionCategoryController : Controller
    {
        private readonly ExamContext _db;
        private readonly ICategoryService _categoryService;
        public QuestionCategoryController(ExamContext db, ICategoryService categoryService)
        {
            _db = db;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            IEnumerable<QuestionCategory> objQuestionCategoryList = _db.QuestionCategories.ToList();
            ViewBag.QuestionCategoryList = _db.QuestionCategories.ToList();

            return View() ;
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Common.QuestionCategory questionCategory)
        {
            if (ModelState.IsValid)
            {
                var categoryModel = new ExamThesis.Common.QuestionCategory()
                {
                    QuestionCategoryName = questionCategory.QuestionCategoryName,
                };

                //_db.QuestionCategories.Add(questionCategory);
                //await _db.SaveChangesAsync();
                //ViewBag.QuestionCategoryList = _db.QuestionCategories.ToList();
                await _categoryService.Create(categoryModel);
                ViewBag.QuestionCategoryList = _db.QuestionCategories.ToList();
            }
            return View("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var QCategoryFromDbFirst = _db.QuestionCategories.Where(u => u.QuestionCategoryId == id).First();

            if (QCategoryFromDbFirst == null)
            {
                return NotFound();
            }
            
            return View(QCategoryFromDbFirst);
        }



        [HttpPut]
        public async Task<IActionResult> Edit(int id,[FromBody]Common.QuestionCategory obj)
        {
       
            if (id == 0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               await _categoryService.Edit(obj,id);
               
                return RedirectToAction("Index"); 
            }
            return BadRequest(ModelState); 
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var QCategoryFromDb = _db.QuestionCategories.Find(id);
         

            if (QCategoryFromDb == null)
            {
                return NotFound();
            }

            return View(QCategoryFromDb);
        }

      
        [HttpDelete]
        public async Task<IActionResult> DeletePost(int id)
        {
            
            await _categoryService.DeleteById(id);
            
                return View("Index");
        }

    }
}
