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
    [Authorize(Roles = UserRoles.Student)]
    public class QuestionCreateController : Controller
    {

        private readonly ExamContext _db;
        private readonly IQuestionService _questionService;

        public QuestionCreateController(ExamContext db, IQuestionService createQuestionService, IQuestionService editQuestionService)
        {
            _db = db;
            _questionService = createQuestionService;
        }

        // GET: /QuestionCreate?categoryId=123&page=1&pageSize=10
        public async Task<IActionResult> Index(int? categoryId, int page = 1, int pageSize = 10)
        {
            if (pageSize <= 0) pageSize = 10;

            var query = _db.Questions
                .AsNoTracking()
                .Include(q => q.Answers)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(q => q.QuestionCategoryId == categoryId.Value);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var questions = await query
                .OrderBy(q => q.QuestionId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _db.QuestionCategories
                .AsNoTracking()
                .OrderBy(c => c.QuestionCategoryName)
                .ToListAsync();

            ViewBag.Categories = new SelectList(categories, "QuestionCategoryId", "QuestionCategoryName", categoryId);
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

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
        public async Task<IActionResult> Create(CreateQuestion question, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(question);

            var createModel = new ExamThesis.Common.CreateQuestion
            {
                QuestionText = question.QuestionText,
                Answers = question.Answers
                    .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                    .Select(a => new ExamThesis.Common.CreateAnswer { Text = a.Text, IsCorrect = a.IsCorrect })
                    .ToList(),
                QuestionPoints = question.QuestionPoints,
                NegativePoints = question.NegativePoints,
                QuestionCategoryId = question.QuestionCategoryId,
                PackageId = question.PackageId,
                ImageType = question.ImageType // set early
            };

            if (imageFile is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await imageFile.CopyToAsync(ms);
                createModel.ImageData = ms.ToArray();

                // If you want to trust the actual file instead of the dropdown, uncomment:
                // var ext = Path.GetExtension(imageFile.FileName);
                // createModel.ImageType = ext;
            }

            Console.WriteLine($"DEBUG Posted ImageType: '{question.ImageType}'");
            Console.WriteLine($"DEBUG Final createModel.ImageType: '{createModel.ImageType}'");

            await _questionService.Create(createModel, imageFile);
            TempData["SuccessMessage"] = "Questions and Answers created successfully.";
            return RedirectToAction("Index");
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
