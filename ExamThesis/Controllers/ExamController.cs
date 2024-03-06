using ExamThesis.Models;
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
            var questionCategories = _db.QuestionCategories.Select(qc => new ExamThesis.Models.QuestionCategory()
            {
                QuestionCategoryId=qc.QuestionCategoryId,
                QuestionCategoryName=qc.QuestionCategoryName,
            }).ToList();

            var model = new CreateExam()
            {
                SelectedCategories = questionCategories
                
             };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExam model)
        
        {
            if (ModelState.IsValid)
            {
                var examModel= new ExamThesis.Common.CreateExam(){
                    ExamName = model.ExamName,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    TotalPoints = model.TotalPoints,
                    SelectedCategories = model.SelectedCategories.Where(sc => sc.IsChecked==true).Select(sc => new Common.QuestionCategory() {
                    QuestionCategoryId=sc.QuestionCategoryId}).ToList()
                    
                };
             await _examService.CreateExam(examModel);

                return RedirectToAction("Index");
            }

            return View(model);
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
        public async Task<IActionResult> Exam(int id)
        {
            ViewBag.ExamId = id;
            var model =await _examService.GetExamQuestionsByExamId(id);
            return base.View(model);
        }
        public async Task<IActionResult> Submit(int id, List<int> selectedAnswers)
        {
            try
            {
                var earnedPoints = await _examService.SubmitExam(id, selectedAnswers);

                var exam = await _db.Exams.FindAsync(id);
                if (exam == null)
                {
                    return NotFound();
                }

                ViewBag.EarnedPoints = earnedPoints;
                ViewBag.TotalPoints = exam.TotalPoints;

                if (earnedPoints >= exam.TotalPoints / 2)
                {
                    ViewBag.Passed = true;
                }
                else
                {
                    ViewBag.Passed = false;
                }

                return View("Result");
            }
            catch (Exception ex)
            {
                // Εδώ μπορείτε να χειριστείτε τυχόν εξαιρέσεις που μπορεί να προκύψουν κατά την υποβολή.
                return BadRequest(ex.Message);
            }
        }
    }

}
