using ExamThesis.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
})
.ConfigureServices((hostContext, services) =>
{
    services.AddControllersWithViews();
    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
        hostContext.Configuration.GetConnectionString("DefaultConnection")));

    services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "OAuth";
    })
    .AddCookie("Cookies")
    .AddOAuth("OAuth", options =>
    {
        options.ClientId = "CLIENT_ID";
        options.ClientSecret = "CLIENT_SECRET";
        options.CallbackPath = new PathString("/callback");
        options.AuthorizationEndpoint = "https://login.it.teithe.gr/authorize";
        options.TokenEndpoint = "https://login.it.teithe.gr/token";
        options.UserInformationEndpoint = "https://api.it.teithe.gr/profile";
        options.SaveTokens = true;
        options.ClaimActions.MapJsonKey("id", "id");
        options.ClaimActions.MapJsonKey("cn", "cn");
        options.ClaimActions.MapJsonKey("title", "title");
    });
})
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Program>();
})
.Build();
 
// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
