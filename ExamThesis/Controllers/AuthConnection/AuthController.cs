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

        [HttpGet]
        public async Task<IActionResult> Callback(string code, string state)
        {

            var token = await GetAccessToken(code);
            var profileResponse = await GetProfile(token.access_token);
            //Αποθήκευση των πληροφοριών του χρήστη στην συνεδρία
            //HttpContext.Session.SetString("UserID", profileResponse.id);
            ViewData.Add("Name", profileResponse.cn);
            //HttpContext.Session.SetString("Email", profileResponse.mail);

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


