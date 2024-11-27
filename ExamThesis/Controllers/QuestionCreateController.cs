using ExamThesis.Storage.Model;
using ExamThesis.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamThesis.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ExamThesis.Common.Model;
using static ExamThesis.Controllers.AuthConnection.AuthController;



namespace ExamThesis.Controllers
{
    [Authorize(Roles = UserRoles.Teacher)]
    public class QuestionCreateController : Controller
    {

        private readonly ExamContext _db;
        private readonly IQuestionService _questionService;

        public QuestionCreateController(ExamContext db, IQuestionService createQuestionService, IQuestionService editQuestionService)
        {
            _db = db;
            _questionService = createQuestionService;
        }

        public IActionResult Index()
        {
            var questions = _db.Questions.Include(q => q.Answers).ToList();
            return View(questions);
        }

        public IActionResult Create()
        {
            var viewModel = new Models.CreateQuestion();
            var questionPackages = _db.QuestionPackages.ToList();
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");
            ViewBag.QuestionPackages = new SelectList(questionPackages, "PackageId", "PackageName");
            return View(viewModel);
        }
        public IActionResult Details(int id)
        {
            var model = _db.Questions
            .Where(q => q.QuestionId == id)
            .Include(q => q.Answers)
            .FirstOrDefault();


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateQuestion question)
        {
            if (ModelState.IsValid)
            {
                var createModel = new ExamThesis.Common.CreateQuestion()
                {
                    QuestionText = question.QuestionText,
                    Answers = question.Answers
                    .Where(answer => !string.IsNullOrEmpty(answer.Text))
                    .Select(answer => new ExamThesis.Common.CreateAnswer
                    {
                        Text = answer.Text,
                        IsCorrect = answer.IsCorrect

                    }).ToList(),
                    QuestionPoints = question.QuestionPoints,
                    NegativePoints = question.NegativePoints,
                    QuestionCategoryId = question.QuestionCategoryId,
                    PackageId = question.PackageId
                };
                await _questionService.Create(createModel);
                TempData["SuccessMessage"] = "Questions and Answers created successfully.";
                return RedirectToAction("Index");
            }
            TempData["FailMessage"] = "Questions and Answers created failed.";

            return View(question);
        }


        [HttpPost]
        public IActionResult AddAnswer()
        {
            // Κώδικας για να προσθέσετε μια νέα απάντηση στο μοντέλο
            var newAnswer = new CreateAnswer();
            // Εδώ πρέπει να προσθέσετε τη νέα απάντηση στο μοντέλο της ερώτησης

            return PartialView("_AnswerPartial", newAnswer);
        }

        public IActionResult Delete(int id)
        {
            var questionFromDb = _db.Questions.Find(id);

            if (questionFromDb == null)
            {
                return NotFound();
            }

            return View(questionFromDb);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _questionService.DeleteById(id);
            return RedirectToAction("Index");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAnswer(string answerText)
        {
            if (await _questionService.DeleteAnswer(answerText))
                return Ok(new { message = "Η απάντηση διαγράφηκε επιτυχώς." });

            return BadRequest(new { message = "Σφάλμα κατά τη διαγραφή της απάντησης." });
        }

    }


}
