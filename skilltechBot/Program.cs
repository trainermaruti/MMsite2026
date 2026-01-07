using SkillTechNavigator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Course Service (Singleton - loads catalog once)
builder.Services.AddSingleton<ICourseService, CourseService>();

// Register Lead Service (Singleton - handles lead capture)
builder.Services.AddSingleton<ILeadService, LeadService>();

// Register HttpClient for Gemini API
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

// Register Gemini service
builder.Services.AddScoped<IGeminiService, GeminiService>();

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
