using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamThesis.Controllers
{
    public class ExamController : Controller
    {
        private readonly ExamContext _db;
        public ExamController(ExamContext db)
        {
            _db = db;
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
        public IActionResult DeleteConfirmed(int id)
        {
            var examsToDelete = _db.Exams.Find(id);

            if (examsToDelete == null)
            {
                return NotFound();
            }

            _db.Exams.Remove(examsToDelete);
            _db.SaveChanges();

            return View(examsToDelete);
        }
        public IActionResult Exam(int id)
        {
            // Λογική για να ανακτήσετε τις ερωτήσεις ανά κατηγορία για την εξέταση με το συγκεκριμένο ID
            var exam = _db.Questions.Where(q => q.ExamId == id).ToList();

            // Κατάλληλη λογική για να εμφανίσετε τις ερωτήσεις στο view

            return View(exam);
        }
    }

}
