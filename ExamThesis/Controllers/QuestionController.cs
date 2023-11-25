using ExamThesis.Data;
using ExamThesis.Models;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Octopus.Client.Model;

namespace ExamThesis.Controllers
{
    public class QuestionController : Controller
    {
      
        private readonly ExamContext _context;
        public QuestionController(ExamContext context)
        {
          
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<ExamThesis.Storage.Model.Question> objQuestionList = _context.Questions.ToList();
            return View(objQuestionList);
        }
        public  IActionResult Create()
        {
            ViewBag.QuestionCategories = _context.QuestionCategories.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamThesis.Models.Question obj)
        {
            ViewBag.QuestionCategories = _context.QuestionCategories.ToList();
            if (ModelState.IsValid)
            {
                var model = new ExamThesis.Storage.Model.Question()
                {
                    NegativePoints = obj.NegativePoints,
                    QuestionPoints = obj.QuestionPoints,
                    QuestionText = obj.QuestionText,   
                    QuestionCategoryId = obj.QuestionCategoryId,    
                    QuestionTypeId = obj.QuestionTypeId,  
                };
                // Εδώ γίνεται η αποθήκευση της ερώτησης στη βάση δεδομένων
                _context.Questions.Add(model);
                await _context.SaveChangesAsync();
                
            }

            // Εάν το ModelState δεν είναι έγκυρο, πρέπει να ξαναφορτώσετε τις κατηγορίες για το dropdown list
           
            return View("Index");
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
