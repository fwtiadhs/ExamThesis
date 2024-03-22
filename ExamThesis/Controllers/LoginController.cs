using ExamThesis.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;


namespace ExamThesis.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult LoginDummy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginDummy(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = DummyUsers.AllUsers.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(model);
            }

            // Αποθήκευση των πληροφοριών του χρήστη στο session
            HttpContext.Session.SetString("UserId", user.Username);
            HttpContext.Session.SetString("EduPersonAffiliation", user.EduPersonAffiliation);

            // Επιτυχής σύνδεση
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Εκκαθάριση του session και αποσύνδεση του χρήστη
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}