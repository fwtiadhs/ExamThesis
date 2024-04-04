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
        public async Task<IActionResult> Create(Models.QuestionCategory questionCategory)
        {
            using (var memoryStream = new MemoryStream())
            {
                
                if (ModelState.IsValid)
                {
                    //await questionCategory.FileData.CopyToAsync(memoryStream);
                    var categoryModel = new ExamThesis.Common.QuestionCategory()
                    {
                        QuestionCategoryName = questionCategory.QuestionCategoryName,
                        //FileData = memoryStream.ToArray()
                    };

                    await _categoryService.Create(categoryModel);
                    ViewBag.QuestionCategoryList = _db.QuestionCategories.ToList();
                }

                return View("Index");
            }
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
        private bool IsFileValid(IFormFile file)
        {
            var allowedExtensions = new[] { ".pcapng", ".pkt", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            return allowedExtensions.Contains(fileExtension);
        }
    }
}
