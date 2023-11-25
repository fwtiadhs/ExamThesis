using ExamThesis.Data;
using ExamThesis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Octopus.Client.Model;

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
        public  IActionResult Create()
        {
            ViewBag.QuestionCategories = _db.QuestionCategories.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question obj)
        {
            ViewBag.QuestionCategories = _db.QuestionCategories.ToList();
            if (ModelState.IsValid)
            {
                // Εδώ γίνεται η αποθήκευση της ερώτησης στη βάση δεδομένων
                _db.Questions.Add(obj);
                await _db.SaveChangesAsync();
                return Ok(obj);
            }

            // Εάν το ModelState δεν είναι έγκυρο, πρέπει να ξαναφορτώσετε τις κατηγορίες για το dropdown list
           
            return View(obj);
        }
        //POST ACTION
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Question obj)
        {
            if (obj.QuestionText == obj.QuestionText.ToString())
            {
                ModelState.AddModelError("Question", "Cannot be the same ");
            }
            if (obj.QuestionPoints == obj.QuestionPoints)
            {
                ModelState.AddModelError("Question", "Cannot be the same ");
            }
            if (obj.NegativePoints == obj.NegativePoints)
            {
                ModelState.AddModelError("Question", "Cannot be the same ");
            }
            QuestionCategory category = _db.QuestionCategories.FirstOrDefault(c => c.QuestionCategoryId == obj.QuestionCategoryId);
            if (category != null)
            {
                // Αν βρείτε την κατηγορία, τότε την αντιστοιχίζετε στο αντικείμενο Question.
                obj.QuestionCategoryId = category.QuestionCategoryId;

                _db.Questions.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");
        }
        */

        /*public async Task<IActionResult> OnPostUploadAsync()
        {
            using (var memoryStream = new MemoryStream())
            {
                await FileUpload.FilePath.CopyToAsync(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    var file = new AppFile()
                    {
                        Content = memoryStream.ToArray()
                    };

                    _db.AppFile.Add(file);

                    await _db.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }

            return View();
        }*/


        // Άλλες μέθοδοι ελέγχου για τη δημιουργία, επεξεργασία και διαγραφή ερωτήσεων
    }
    
   


}   
