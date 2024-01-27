using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using System.Web.WebPages.Html;
using Microsoft.EntityFrameworkCore;
using ExamThesis.Models;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace ExamThesis.Controllers
{
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
            //viewModel.Categories = _db.QuestionCategories
            // .Select(c => new SelectListItem
            // {
            //     Value = c.QuestionCategoryId.ToString(),
            //     Text = c.QuestionCategoryName
            // })
            // .ToList();
            ViewBag.QuestionCategories = new SelectList( _db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");
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
                            
                        }).ToList(),
                            QuestionPoints = viewModel.QuestionPoints,
                            NegativePoints = viewModel.NegativePoints,
                            QuestionCategoryId = viewModel.QuestionCategoryId
                };

                _db.Questions.Add(question);
                _db.SaveChanges();

                return RedirectToAction("Index"); // Ή οποιαδήποτε άλλη δράση που θέλετε να πάει μετά τη δημιουργία
            }

            // Αν υπάρχουν λάθη, επιστρέφετε στο View με τα λάθη
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddAnswer()
        {
            // Κώδικας για να προσθέσετε μια νέα απάντηση στο μοντέλο
            var newAnswer = new CreateAnswer();
            // Εδώ πρέπει να προσθέσετε τη νέα απάντηση στο μοντέλο της ερώτησης

            return PartialView("_AnswerPartial", newAnswer);
        }

    }


}
