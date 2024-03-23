﻿using ClosedXML.Excel;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExam model)
        
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model error: {error.ErrorMessage}");
                }
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var examModel= new ExamThesis.Common.CreateExam(){
                    ExamName = model.ExamName,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    TotalPoints = model.TotalPoints,
                    PassGrade = model.PassGrade,
                    SelectedCategories = model.SelectedCategories.Where(sc => sc.IsChecked==true).Select(sc => new Common.QuestionCategory() {
                    QuestionCategoryId=sc.QuestionCategoryId}).ToList()
                    
                };
             await _examService.CreateExam(examModel);

                return RedirectToAction("Index");
            }

            return View(model);
        }
        public IActionResult Delete(int id)
        {
            var examFromDb = _db.Exams.Find(id);

            if (examFromDb == null)
            {
                return NotFound();
            }

            return View(examFromDb);
        }
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  DeleteConfirmed(int id)
        {
           await _examService.DeleteByExamId(id);

            return Index();
        }
        public async Task<IActionResult> Exam(int id)
        {
            if (IsUserAlreadyParticipated(id))
            {
                TempData["ErrorMessage"] = "Έχετε ήδη ολοκληρώσει αυτήν την εξέταση.";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ExamId = id;
                var model = await _examService.GetExamQuestionsByExamId(id);
                return base.View(model);
            }
        }
        public bool IsUserAlreadyParticipated(int examId)
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
        [HttpGet]
        public IActionResult CheckUserParticipation(int examId)
        {
            // Εκτελέστε τον έλεγχο για τη συμμετοχή του χρήστη
            var isParticipated = IsUserAlreadyParticipated(examId);

            // Επιστροφή αποτελέσματος σε μορφή JSON
            return Json(new { isParticipated });
        }

        public async Task<IActionResult> Submit(int id, List<int> selectedAnswers,string studentId)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst("UserId");
            studentId = userIdClaim.Value; 
            try
            {
                var earnedPoints = await _examService.SubmitExam(id, selectedAnswers,studentId);
                
                var exam = await _db.Exams.FindAsync(id);
                if (exam == null)
                {
                    return NotFound();
                }

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
                // Εδώ μπορείτε να χειριστείτε τυχόν εξαιρέσεις που μπορεί να προκύψουν κατά την υποβολή.
                return BadRequest(ex.Message);
            }
        }
        public IActionResult ExportExamResultsToExcel()
        {
            var examResults = _db.ExamResults.ToList(); 
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ExamResults");

            worksheet.Cell(1, 1).Value = "ExamResultId";
            worksheet.Cell(1, 2).Value = "ExamId";
            worksheet.Cell(1, 3).Value = "Grade";

            for (int i = 0; i < examResults.Count; i++)
            {
                var examResult = examResults[i];
                worksheet.Cell(i + 2, 1).Value = examResult.ExamResultId;
                worksheet.Cell(i + 2, 2).Value = examResult.ExamId;
                worksheet.Cell(i + 2, 3).Value = examResult.Grade;
            }

            var fileName = "ExamResultsExport.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            workbook.SaveAs(filePath);

            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(filePath, mimeType, fileName);
        }
    }

}
