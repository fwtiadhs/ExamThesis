using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using ExamThesis.Storage.Model;
using ExamThesis.Storage;
using ExamThesis.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(500);
        options.SlidingExpiration = true;
        options.Cookie.Name = "UserCookieName";
        options.LoginPath = "/Auth/Login"; // Η διεύθυνση για τη σελίδα εισόδου
        options.LogoutPath = "/Auth/Logout"; // Η διεύθυνση για τη σελίδα εξόδου
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Cookie.HttpOnly = true;
    });
builder.Services.AddDbContext<ExamContext>(options => options.UseSqlServer(
  builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddTransient<IExamService, ExamService>();
builder.Services.AddTransient<IExamCategoryService, ExamCategoryService>();
builder.Services.AddTransient<ICreateQuestionService, CreateQuestionService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
await Seed();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();

app.UseSession();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task Seed() {
using(var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var examContext = services.GetRequiredService<ExamContext>();
        examContext.Database.Migrate();
        await SeedData.Seed(examContext);
    }
}