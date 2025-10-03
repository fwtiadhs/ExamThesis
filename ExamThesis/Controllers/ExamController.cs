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

        public async Task<IActionResult> Exam(int id)
        {
            if (IsUserAlreadyParticipated(id))
            {
                TempData["ErrorMessage"] = "Έχετε ήδη ολοκληρώσει αυτήν την εξέταση.";
                return RedirectToAction("Index");
            }
            //else if (!CanStartExam(id))
            //{
            //    TempData["ErrorMessage"] = "Δεν έχει ξεκινήσει η εξέταση!";
            //    return RedirectToAction("Index");
            //}
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
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst("UserId");
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;

                var examResult = _db.ExamResults.Any(er => er.ExamId == examId && er.StudentId == userId);

                return examResult;
            }
            else
            {
                return false;
            }

        }
        private bool CanStartExam(int examId)
        {
            var exam = _db.Exams.First(er => er.ExamId == examId);
            return DateTime.Now >= exam.StartTime || DateTime.Now <exam.EndTime;
        }
        [HttpGet]

        public IActionResult CheckUserParticipation(int examId)
        {
            // Εκτελέστε τον έλεγχο για τη συμμετοχή του χρήστη
            var isParticipated = IsUserAlreadyParticipated(examId);

            // Επιστροφή αποτελέσματος σε μορφή JSON
            return Json(new { isParticipated });
        }

        public async Task<IActionResult> Submit(int id, List<int> selectedAnswers,List<int> selectedQuestions,string studentId)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst("UserId");
            studentId = userIdClaim.Value; 
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

                return View("Result");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = UserRoles.Teacher)]
        public IActionResult ExportExamResultsToExcel(int id)
        {
            var examResults = _db.ExamResults.Where(er => er.ExamId == id).ToList();
            using (var workbook = new XLWorkbook())
            {


                var worksheet = workbook.Worksheets.Add("ExamResults");

                worksheet.Cell(1, 1).Value = "StudentId";
                worksheet.Cell(1, 2).Value = "ExamId";
                worksheet.Cell(1, 3).Value = "Grade";

                for (int i = 0; i < examResults.Count; i++)
                {
                    var examResult = examResults[i];
                    worksheet.Cell(i + 2, 1).Value = examResult.StudentId;
                    worksheet.Cell(i + 2, 2).Value = examResult.ExamId;
                    worksheet.Cell(i + 2, 3).Value = examResult.Grade;
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
