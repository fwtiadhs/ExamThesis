using ExamThesis.Storage.Model;
using ExamThesis.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamThesis.Models;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace ExamThesis.Controllers
{
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
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExamThesis.Common.CreateQuestion question)
        {
            if (ModelState.IsValid)
            {

                await _questionService.Create(question);

                return RedirectToAction("Index");
            }

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


        public IActionResult Edit(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var questionFromDb = _db.Questions.Where(q => q.QuestionId == id).Include(q => q.Answers).First();

            if (questionFromDb == null)
            {
                return NotFound();
            }

            // Δημιουργούμε ένα αντικείμενο CreateQuestion για το View
            var viewModel = new CreateQuestion
            {
                QuestionId = questionFromDb.QuestionId,
                QuestionText = questionFromDb.QuestionText,
                QuestionPoints = questionFromDb.QuestionPoints,
                NegativePoints = questionFromDb.NegativePoints,
                QuestionCategoryId = questionFromDb.QuestionCategoryId,
                Answers = questionFromDb.Answers.Select(answer => new CreateAnswer
                {
                    Text = answer.Text,
                    IsCorrect = answer.IsCorrect ?? false
                }).ToList()
                // Προσθέστε τα υπόλοιπα πεδία αν υπάρχουν
            };

            // Προσθέτουμε τα επιλέξιμα QuestionCategories στο ViewBag
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");

            return View(viewModel);
        }


        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] ExamThesis.Common.CreateQuestion editQuestionService)
        {
            if (ModelState.IsValid)
            {

                await _questionService.Edit(editQuestionService);

                return RedirectToAction("Index");
            }

            return BadRequest(ModelState);
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
