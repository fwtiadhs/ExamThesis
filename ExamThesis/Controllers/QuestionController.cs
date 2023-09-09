using ExamThesis.Data;
using ExamThesis.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamThesis.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuestionController(ApplicationDbContext db)
        {
            _db = db;
        }
    

        public IActionResult Index()
        {
            IEnumerable<Question> objQuestionList = _db.Questions.ToList();
            return View(objQuestionList);
        }

        // Άλλες μέθοδοι ελέγχου για τη δημιουργία, επεξεργασία και διαγραφή ερωτήσεων
    }

}
