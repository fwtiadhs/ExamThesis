using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamThesis.Controllers
{
    // QuestionCreateController.cs
    public class QuestionCreateController : Controller
    {
        private readonly ExamContext _db;

        public QuestionCreateController(ExamContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var questions = _db.Questions.Include(q => q.Answers).ToList();
            return View(questions);
        }
        public IActionResult Create()
        {
            var viewModel = new CreateQuestion();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(CreateQuestion viewModel)
        {
            if (ModelState.IsValid)
            {
                var question = new Question
                {
                    QuestionText = viewModel.QuestionText,
                    Answers = viewModel.Answers
                        .Where(answer => !string.IsNullOrEmpty(answer.Text))
                        .Select(answer => new Answer
                        {
                            Text = answer.Text,
                            IsCorrect = answer.IsCorrect
                        }).ToList()
                };

                _db.Questions.Add(question);
                _db.SaveChanges();

                return RedirectToAction("Index"); // Ή οποιαδήποτε άλλη δράση που θέλετε να πάει μετά τη δημιουργία
            }

            // Αν υπάρχουν λάθη, επιστρέφετε στο View με τα λάθη
            return View(viewModel);
        }
       
    }


}
