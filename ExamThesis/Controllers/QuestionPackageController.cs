using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static ExamThesis.Controllers.AuthConnection.AuthController;

namespace ExamThesis.Controllers
{
    [Authorize(Roles = UserRoles.Student)]
    public class QuestionPackageController : Controller
    {
        private readonly ExamContext _db;
        private readonly IQuestionService _questionService;
        public QuestionPackageController(ExamContext db, IQuestionService createQuestionService)
        {
            _db = db;
            _questionService = createQuestionService;
        }
        public IActionResult Index()
        {
            var objQuestionPackageList = _db.QuestionPackages
                .Include(qp => qp.QuestionCategory)                 // <-- load category
                .Include(qp => qp.QuestionsInPackages)
                    .ThenInclude(qip => qip.Question)
                .AsNoTracking()
                .ToList();

            // Natural sort packages by name (e.g., ping 1, ping 2, ping 10)
            objQuestionPackageList = objQuestionPackageList
                .OrderBy(p => p.PackageName, NaturalStringComparer.Instance)
                .ToList();

            // Natural sort questions inside each package by QuestionText
            foreach (var pkg in objQuestionPackageList)
            {
                if (pkg?.QuestionsInPackages != null)
                {
                    pkg.QuestionsInPackages = pkg.QuestionsInPackages
                        .OrderBy(qip => qip!.Question?.QuestionText, NaturalStringComparer.Instance)
                        .ToList();
                }
            }

            ViewBag.QuestionPackagesList = objQuestionPackageList;
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");
            return View(objQuestionPackageList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.QuestionCategories = new SelectList(_db.QuestionCategories, "QuestionCategoryId", "QuestionCategoryName");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Models.QuestionPackage package)
        {

            using (var memoryStream = new MemoryStream())
            {

                if (ModelState.IsValid)
                {
                    byte[]? fileBytes = null;
                    string? fileType = null;

                    // Only process file if one was actually uploaded
                    if (package.FileData != null && package.FileData.Length > 0)
                    {
                        await package.FileData.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray();

                        // Keep your existing variable usage; infer extension if FileType not bound
                        if (string.IsNullOrWhiteSpace(package.FileType))
                        {
                            fileType = Path.GetExtension(package.FileData.FileName)?.ToLowerInvariant();
                        }
                        else
                        {
                            fileType = package.FileType;
                        }
                    }

                    var pacageModel = new QuestionPackage()
                    {
                        PackageName = package.PackageName,
                        QuestionCategoryId = package.QuestionCategoryId,
                        FileData = fileBytes,   // null when no file uploaded
                        FileType = fileType     // null when no file uploaded
                    };

                    await _questionService.CreatePackage(pacageModel);
                    ViewBag.QuestionCategoryList = _db.QuestionCategories.ToList();
                    return RedirectToAction("Index");
                }



                return RedirectToAction("Index");
            }

        }
        public async Task<IActionResult> Download(int id)
        {
            // Βρείτε το αντικείμενο QuestionPackage στη βάση δεδομένων με βάση το Id
            var package = await _questionService.GetPackageById(id);

            // Εάν το αντικείμενο δε βρεθεί, επιστρέψτε μια κατάλληλη απόκριση
            if (package == null)
            {
                return NotFound();
            }

            byte[] fileData = package.FileData;
            string fileExtension = string.Empty;

            // Έλεγχος του τύπου αρχείου
            switch (package.FileType)
            {
                case ".pdf":
                    fileExtension = ".pdf";
                    break;
                case ".pcapng":
                    fileExtension = ".pcapng";
                    break;
                case ".pkt":
                    fileExtension = ".pkt";
                    break;
                case ".txt":
                    fileExtension = ".txt";
                    break;
                default:
                    return BadRequest("Unsupported file type.");
            }
            // Δημιουργία του ονόματος αρχείου με τη σωστή επέκταση
            string fileName = package.PackageName + fileExtension;

            // Δημιουργήστε έναν FileContentResult
            var fileContentResult = new FileContentResult(fileData, "application/octet-stream")
            {
                FileDownloadName = fileName
            };

            // Επιστρέψτε το αρχείο ως απόκριση
            return fileContentResult;
        }
    
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _questionService.DeletePackage(id);
            return RedirectToAction("Index");
        }

        private bool IsFileValid(IFormFile file)
        {
            var allowedExtensions = new[] { ".pcapng", ".pkt", ".pdf", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            return allowedExtensions.Contains(fileExtension);
        }
    }

    internal sealed class NaturalStringComparer : IComparer<string?>
    {
        public static readonly NaturalStringComparer Instance = new();

        public int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            var rx = Tokenize(x);
            var ry = Tokenize(y);
            int i = 0;
            for (; i < rx.Count && i < ry.Count; i++)
            {
                var a = rx[i];
                var b = ry[i];
                int cmp;
                if (a.IsNumber && b.IsNumber)
                {
                    cmp = a.Number!.Value.CompareTo(b.Number!.Value);
                }
                else
                {
                    cmp = string.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase);
                }
                if (cmp != 0) return cmp;
            }
            return rx.Count.CompareTo(ry.Count);
        }

        private static List<Token> Tokenize(string s)
        {
            var list = new List<Token>();
            var matches = Regex.Matches(s, "(\\d+)|(\\D+)");
            foreach (Match m in matches)
            {
                if (m.Groups[1].Success)
                {
                    if (long.TryParse(m.Value, out var n)) list.Add(Token.Num(n));
                    else list.Add(Token.Txt(m.Value));
                }
                else
                {
                    list.Add(Token.Txt(m.Value));
                }
            }
            return list;
        }

        private readonly struct Token
        {
            public bool IsNumber { get; }
            public long? Number { get; }
            public string Text { get; }
            private Token(long number) { IsNumber = true; Number = number; Text = string.Empty; }
            private Token(string text) { IsNumber = false; Number = null; Text = text; }
            public static Token Num(long n) => new(n);
            public static Token Txt(string t) => new(t);
        }
    }
}
