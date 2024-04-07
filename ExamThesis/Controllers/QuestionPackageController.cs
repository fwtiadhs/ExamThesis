using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamThesis.Controllers
{
    [Authorize]
    public class QuestionPackageController : Controller
    {
        private readonly ExamContext _db;
        private readonly IQuestionService _questionService;
        public QuestionPackageController(ExamContext db, IQuestionService createQuestionService)
        {
            _db = db;
            _questionService = createQuestionService;
        }
        public IActionResult Index()
        {
            IEnumerable<QuestionPackage> objQuestionPackageList = _db.QuestionPackages.ToList();
            ViewBag.QuestionPackagesList = _db.QuestionPackages.ToList();
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");
            return View(objQuestionPackageList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Models.QuestionPackage package)
        {

            using (var memoryStream = new MemoryStream())
            {
                
                if (ModelState.IsValid)
                {
                    await package.FileData.CopyToAsync(memoryStream);
                    var pacageModel = new QuestionPackage()
                    {
                        PackageName = package.PackageName,
                        QuestionCategoryId = package.QuestionCategoryId,
                        FileData = memoryStream.ToArray()
                    };

                    await _questionService.CreatePackage(pacageModel);
                    ViewBag.QuestionCategoryList = _db.QuestionCategories.ToList();
                    return RedirectToAction("Index");
                }


               
                return RedirectToAction("Index");
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _questionService.DeletePackage(id);
            return RedirectToAction("Index");
        }

        private bool IsFileValid(IFormFile file)
        {
            var allowedExtensions = new[] { ".pcapng", ".pkt", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            return allowedExtensions.Contains(fileExtension);
        }
    }
}
