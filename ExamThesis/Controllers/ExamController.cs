using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExamThesis.Controllers
{
    public class ExamController : Controller
    {
        private readonly ExamContext _db;
        private readonly IExamService _examService;
        public ExamController(ExamContext db, IExamService examService)
        {
            _db = db;
            _examService = examService;
        }
        public IActionResult Index()
        {
            var exams = _db.Exams.ToList();
            return View(exams);
        }
        public IActionResult Create()
        {
            // Λάβετε και περάστε όλες τις κατηγορίες ερωτήσεων στην προβολή
            var questionCategories = _db.QuestionCategories.ToList();
            ViewBag.QuestionCategories = questionCategories;
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Exam viewModel)
        {
            if (ModelState.IsValid)
            {
                var exam = new Exam
                {
                    ExamName = viewModel.ExamName,
                    StartTime = viewModel.StartTime,
                    EndTime = viewModel.EndTime,
                    TotalPoints = viewModel.TotalPoints,
                    QuestionCategoryId = viewModel.QuestionCategoryId
                };

                _db.Exams.Add(exam);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }
        public IActionResult Delete(int id)
        {
            var examFromDb = _db.Exams.Find(id);

            if (examFromDb == null)
            {
                return NotFound();
            }

            return View(examFromDb);
        }
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  DeleteConfirmed(int id)
        {
           await _examService.DeleteByExamId(id);

            return Index();
        }
        public IActionResult Exam(int id)
        {
           return View(_examService.GetExamQuestionsByExamId);
        }
    }

}
