using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExamThesis.Models;
using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;
using static ExamThesis.Controllers.AuthConnection.AuthController;

namespace ExamThesis.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        private readonly ExamContext _db;
        private readonly IExamService _examService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ExamController(ExamContext db, IExamService examService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _examService = examService;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public IActionResult Index()
        {
            var exams = _db.Exams.ToList();
            return View(exams);
        }
        [Authorize(Roles = UserRoles.Student)]
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
        [Authorize(Roles = UserRoles.Student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExam model)
        
        {
            var existingExam = _db.Exams.FirstOrDefault(e => e.ExamName == model.ExamName);
            if (existingExam != null)
            {
                ModelState.AddModelError("ExamName", "Υπάρχει ήδη εξέταση με αυτό το όνομα.");
                return View(model);
            }
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "Η ώρα λήξης πρέπει να είναι μεταγενέστερη της ώρας έναρξης.");
            }
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model error: {error.ErrorMessage}");
                }
                return View(model);
            }
            ViewBag.ShowGrade = model.ShowGrade;
            if (ModelState.IsValid)
            {
                var examModel= new ExamThesis.Common.CreateExam(){
                    ExamName = model.ExamName,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    TotalPoints = model.TotalPoints,
                    PassGrade = model.PassGrade,
                    ShowGrade = model.ShowGrade,
                    SelectedCategories = model.SelectedCategories.Where(sc => sc.IsChecked==true).Select(sc => new Common.QuestionCategory() {
                    QuestionCategoryId=sc.QuestionCategoryId}).ToList()
                    
                };
             await _examService.CreateExam(examModel);

                return RedirectToAction("Index");
            }

            return View(model);
        }
        [Authorize(Roles = UserRoles.Student)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var examFromDb = _db.Exams.Find(id);

            if (examFromDb == null)
            {
                return NotFound();
            }

            return View(examFromDb);
        }
        [Authorize(Roles = UserRoles.Student)]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _examService.DeleteByExamId(id);
            TempData["SuccessMessage"] = "Exam deleted successfully.";
            return RedirectToAction("Index");
        }

        // Prevent cached back navigation to the exam page
        [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Teacher}")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<IActionResult> Exam(int id)
        {
            if (IsUserAlreadyParticipated(id))
            {
                TempData["ErrorMessage"] = "Έχετε ήδη ολοκληρώσει αυτήν την εξέταση.";
                return RedirectToAction("Index");
            }
            else if (!CanStartExam(id))
            {
                TempData["ErrorMessage"] = "Δεν έχει ξεκινήσει η εξέταση!";
                return RedirectToAction("Index");
            }
            else
            {
                var exam = await _db.Exams.FindAsync(id);
                if (exam != null)
                {
                    ViewBag.EndTime = exam.EndTime.ToString("HH:mm:ss");
                    ViewBag.StartTime = exam.StartTime.ToString("HH:mm:ss");
                }
                ViewBag.ExamId = id;
                var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                var userIdClaim = claimsIdentity?.FindFirst("UserId")?.Value;
                var model = await _examService.GetExamQuestionsByExamId(id, userIdClaim);
                return base.View(model);
            }
        }
        private bool IsUserAlreadyParticipated(int examId)
        {
            // Prefer AM; fallback to UID if AM is missing
            var am = HttpContext.Session.GetString("AM") 
                     ?? (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity)?.FindFirst("AM")?.Value;
            var uid = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity)?.FindFirst("UserId")?.Value;

            var identifier = am ?? uid;
            if (!string.IsNullOrEmpty(identifier))
            {
                var examResult = _db.ExamResults.Any(er => er.ExamId == examId && er.StudentId == identifier);
                return examResult;
            }
            else
            {
                return false;
            }

        }
        private bool CanStartExam(int examId)
        {
            var exam = _db.Exams.AsNoTracking().FirstOrDefault(er => er.ExamId == examId);
            if (exam == null) return false;

            // If you store UTC times, use DateTime.UtcNow consistently.
            var now = DateTime.Now;

            // Allow only within the exam window
            return now >= exam.StartTime && now <= exam.EndTime;
        }
        [HttpGet]

        public IActionResult CheckUserParticipation(int examId)
        {
            // Εκτελέστε τον έλεγχο για τη συμμετοχή του χρήστη
            var isParticipated = IsUserAlreadyParticipated(examId);

            // Επιστροφή αποτελέσματος σε μορφή JSON
            return Json(new { isParticipated });
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public async Task<IActionResult> Submit(int id, List<int> selectedAnswers,List<int> selectedQuestions,string studentId)
        {
            // Prefer AM; fallback to UID if AM is missing
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var am = HttpContext.Session.GetString("AM") ?? claimsIdentity?.FindFirst("AM")?.Value;
            var uid = claimsIdentity?.FindFirst("UserId")?.Value;

            studentId = am ?? uid;

            try
            {
                var earnedPoints = await _examService.SubmitExam(id, selectedAnswers,studentId,selectedQuestions);
                
                var exam = await _db.Exams.FindAsync(id);
                if (exam == null)
                {
                    return NotFound();
                }
                ViewBag.ShowGrade = exam.ShowGrade;
                ViewBag.EarnedPoints = earnedPoints;
                ViewBag.TotalPoints = exam.TotalPoints;
                ViewBag.PassGrade = exam.PassGrade;

                if (earnedPoints >= exam.PassGrade)
                {
                    ViewBag.Passed = true;
                }
                else
                {
                    ViewBag.Passed = false;
                }
                Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";


                return View("Result");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = UserRoles.Student)]
        public IActionResult ExportExamResultsToExcel(int id)
        {
            var examResults = _db.ExamResults.Where(er => er.ExamId == id).ToList();
            using (var workbook = new XLWorkbook())
            {


                var worksheet = workbook.Worksheets.Add("ExamResults");

                worksheet.Cell(1, 1).Value = "StudentId";
                worksheet.Cell(1, 2).Value = "Grade";
                worksheet.Cell(1, 3).Value = "ExamId";

                for (int i = 0; i < examResults.Count; i++)
                {
                    var examResult = examResults[i];
                    worksheet.Cell(i + 2, 1).Value = examResult.StudentId; // now AM if present
                    worksheet.Cell(i + 2, 2).Value = examResult.Grade;
                    worksheet.Cell(i + 2, 3).Value = examResult.ExamId;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = "ExamResultsExport.xlsx";
                    return File(content, mimeType, fileName);
                }

            }
        }
    }

}
