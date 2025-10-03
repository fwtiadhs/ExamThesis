using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            IEnumerable<QuestionPackage> objQuestionPackageList = _db.QuestionPackages.ToList();
            ViewBag.QuestionPackagesList = _db.QuestionPackages.ToList();
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
            var allowedExtensions = new[] { ".pcapng", ".pkt", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            return allowedExtensions.Contains(fileExtension);
        }
    }
}
