using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
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
        //public IActionResult Edit(int? id)
        //{
        //    var viewModel = new CreateQuestion();

        //    ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    var QCategoryFromDbFirst = _db.Questions.FirstOrDefault(u => u.QuestionId == id);

        //    if (QCategoryFromDbFirst == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(QCategoryFromDbFirst);
        //}

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var questionFromDb = _db.Questions.FirstOrDefault(q => q.QuestionId == id);

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
                // Προσθέστε τα υπόλοιπα πεδία αν υπάρχουν
            };

            // Προσθέτουμε τα επιλέξιμα QuestionCategories στο ViewBag
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");

            return View(viewModel);
        }


        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromBody] CreateQuestion viewModel)
        {
            if (ModelState.IsValid)
            {
                var question = _db.Questions
                    .Include(q => q.Answers)
                    .FirstOrDefault(q => q.QuestionId == viewModel.QuestionId);

                if (question == null)
                {
                    return NotFound();
                }

                // Ενημέρωση των πεδίων της ερώτησης με τα δεδομένα από το viewModel
                question.QuestionText = viewModel.QuestionText;
                question.Answers = viewModel.Answers
                    .Where(answer => !string.IsNullOrEmpty(answer.Text))
                    .Select(answer => new Answer
                    {
                        Text = answer.Text,
                        IsCorrect = answer.IsCorrect
                    }).ToList();
                question.QuestionPoints = viewModel.QuestionPoints;
                question.NegativePoints = viewModel.NegativePoints;
                question.QuestionCategoryId = viewModel.QuestionCategoryId;
               
                _db.Questions.Update(question);
                _db.SaveChanges();

                return View(question);
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
        public IActionResult DeleteConfirmed(int id)
        {
            var questionToDelete = _db.Questions.Find(id);

            if (questionToDelete == null)
            {
                return NotFound();
            }

            // Πρέπει να διαγράψουμε τις σχετικές απαντήσεις πρώτα
            var answersToDelete = _db.Answers.Where(a => a.QuestionId == id);
            _db.Answers.RemoveRange(answersToDelete);

            _db.Questions.Remove(questionToDelete);
            _db.SaveChanges();

            return View(); 
        }




    }


}
