using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamThesis.Controllers
{
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
        public async Task<IActionResult> Create(QuestionPackage package)
        {

            if (ModelState.IsValid)
            {

                await _questionService.CreatePackage(package);
                return RedirectToAction("Index");
            }
            //ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");

            return View();
        }
    }
}
