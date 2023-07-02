using ExamThesis.Models.AuthModel;
using ExamThesis.Models.UserInfo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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
            /*if (HttpContext.Request.Query.TryGetValue("error", out var error) && HttpContext.Request.Query["state"] == HttpContext.Session.Id)
            {
                ViewBag.Message = "Αρνηθήκατε την σύνδεση.";
            }
            */

            return Redirect(GetUrl());
        }
        private string GetUrl()
        {
            var url = $"https://login.iee.ihu.gr/login?client_id={CLIENT_ID}&response_type=code&scope=profile&state=12345&redirect_uri=https://localhost:7134/Auth/Callback";
            return url;
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string code, string state)
        {

            //var response = await GetAccessToken(code);
            var response = await GetAccessTokenn(code);


            return RedirectToAction("Index", "Home");
        }



        private async Task<string> GetAccessToken(string code)
        {
            var InfoUrl = $"https://api.it.teithe.gr/profile?access_token={code}";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(InfoUrl, UriKind.Absolute),
                Method = HttpMethod.Get

            };

            // request.Headers = new MediaTypeHeaderValue("application/json");
            //request.Headers.Add("x-access-token", code);

            var tokenResponse = await httpClient.SendAsync(request);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<TokenResponse>(tokenContent);

            return tokenContent;

            /*if (token?.error != null)
            {
                ViewBag.Message = token.error.message;
            }
            else
            {
                var infoResponse = await httpClient.GetAsync(InfoUrl + token.access_token);
                var infoContent = await infoResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(infoContent);

                if (userInfo?.Cn != null)
                {
                    // Αποθήκευση των πληροφοριών του χρήστη στην συνεδρία
                    HttpContext.Session.SetString("UserID", userInfo.Id);
                    HttpContext.Session.SetString("Name", userInfo.Cn);
                    HttpContext.Session.SetString("Email", userInfo.Email);
                }
            }*/


            // Υπόλοιπος κώδικας ελεγκτή
        }

        private async Task<string> GetAccessTokenn(string code)
        {
            var TokenUrl = $"https://login.iee.ihu.gr/token?code={code}";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(TokenUrl, UriKind.Absolute),
                Method = HttpMethod.Post

            };

            // request.Headers = new MediaTypeHeaderValue("application/json");
            //request.Headers.Add("x-access-token", code);

            var tokenResponse = await httpClient.SendAsync(request);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<TokenResponse>(tokenContent);

            return tokenContent;

           
        }
    }
}
