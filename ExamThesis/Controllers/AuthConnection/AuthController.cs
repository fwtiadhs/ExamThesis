using ExamThesis.Models.AuthModel;
using ExamThesis.Models.UserInfo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static System.Net.WebRequestMethods;
using ExamThesis.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;

namespace ExamThesis.Controllers.AuthConnection
{

    public class AuthController : Controller
    {
        private const string CLIENT_ID = "64a1b7b4480c2202c1f70d73";
        private const string CLIENT_SECRET = "0yppvt1e34j5hmrd1v909zmmuhu6w4gv5bi3qhfbexmrbs7h5p";
        private const string TokenUrl = "https://login.iee.ihu.gr/token";

       

        [HttpGet]
        public IActionResult Index()
        {
            return Redirect(GetUrl());
        }
        private string GetUrl()
        {
            var url = $"https://login.iee.ihu.gr/login?client_id={CLIENT_ID}&response_type=code&scope=profile&state=12345&redirect_uri=https://localhost:7134/Auth/Callback";
            return url;
        }
        public static class UserRoles
        {
            public const string Teacher = "teacher";
            public const string Student = "student";
            // Προσθέστε άλλους ρόλους εδώ αν χρειάζεται
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string code, string state)
        {

            var token = await GetAccessToken(code);
            var profileResponse = await GetProfile(token.access_token);
            //Αποθήκευση των πληροφοριών του χρήστη στην συνεδρία
            ViewData.Add("Name", profileResponse.cn);
            ViewData.Add("UserId", profileResponse.uid);
            HttpContext.Session.SetString("UserId", profileResponse.uid);
           // HttpContext.Session.SetString("EduPersonAffiliation", profileResponse.eduPersonAffiliation);


            //HttpContext.Session.SetString("Email", profileResponse.mail);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, profileResponse.cn),
                new Claim(ClaimTypes.NameIdentifier, profileResponse.id),
                new Claim("UserId", profileResponse.uid),
                new Claim("Edu", profileResponse.eduPersonAffiliation)
            };

            if (profileResponse.eduPersonAffiliation == "teacher")
            {
                claims.Add(new Claim(ClaimTypes.Role, UserRoles.Teacher));
            }
            else if(profileResponse.eduPersonAffiliation == "student") 
            {
                claims.Add(new Claim(ClaimTypes.Role, UserRoles.Student));
            }
            // Δημιουργήστε έναν ClaimsIdentity με τους ισχυρισμούς
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Κάντε το Sign In με τον ClaimsPrincipal
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
            return RedirectToAction("Index", "Home", new { userId = profileResponse.uid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Εκκαθαρίστε τα δεδομένα του χρήστη από τη συνεδρία
            HttpContext.Session.Clear();
            Response.Cookies.Delete("UserID");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Εκτελέστε τη διαδικασία αποσύνδεσης του χρήστη από την εξωτερική πηγή αυθεντικοποίησης (εάν απαιτείται)

            // Επιστροφή στην αρχική σελίδα ή σε μια συγκεκριμένη σελίδα μετά την αποσύνδεση
            return RedirectToAction("Index", "Home");
        }

        private async Task<TokenResponse> GetAccessToken(string code)
        {
            var TokenUrl = $"https://login.iee.ihu.gr/token";
            var httpClient = new HttpClient();

            var payload = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "client_id", CLIENT_ID },
                { "client_secret", CLIENT_SECRET },
                { "redirect_uri", @"https://localhost:7134/Auth/Callback" },
                { "code", code }
            };

            var stringContent = JsonConvert.SerializeObject(payload);

            var tokenResponse = await httpClient.PostAsync(TokenUrl, new StringContent(stringContent, Encoding.UTF8,
                "application/json"));
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<TokenResponse>(tokenContent);

            return token;


        }
        private async Task<ProfileResponse> GetProfile(string token)
        {
            var TokenUrl = $"https://api.iee.ihu.gr/profile";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-access-token", token);
            var response = await httpClient.GetAsync(TokenUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            var profile = JsonSerializer.Deserialize<ProfileResponse>(responseContent);


            return profile;

        }

    }
}


