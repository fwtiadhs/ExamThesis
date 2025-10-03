using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Octopus.Client.Model;
using static ExamThesis.Controllers.AuthConnection.AuthController;

namespace ExamThesis.Controllers
{
    [Authorize(Roles = UserRoles.Student)]
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
            var questionCategories = _db.QuestionCategories.ToList();

            if (questionCategories == null)
            {
                // Εάν δεν υπάρχουν κατηγορίες ερωτήσεων, επιστρέφουμε κατάλληλο μήνυμα
                ViewBag.ErrorMessage = "No question categories found.";
                return View();
            }

            return View(questionCategories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.QuestionCategory questionCategory)
        {
            if (ModelState.IsValid)
            {
                var categoryModel = new ExamThesis.Common.QuestionCategory()
                {
                    QuestionCategoryName = questionCategory.QuestionCategoryName,
                };

                await _categoryService.Create(categoryModel);

                TempData["SuccessMessage"] = "Question category created successfully.";
                return RedirectToAction("Index");
            }
            TempData["FailMessage"] = "Question category created failed.";
            return View(questionCategory);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _categoryService.DeleteById(id);
            return Ok(new { success = true });
        }

    }
}
