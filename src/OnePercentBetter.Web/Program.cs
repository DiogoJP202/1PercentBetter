using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Identity;
using OnePercentBetter.Web.Options;
using OnePercentBetter.Web.Services;
using Resend;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(3);
});

builder.Services.AddMemoryCache();
builder.Services.Configure<ResendSettings>(builder.Configuration.GetSection(ResendSettings.SectionName));
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = builder.Configuration.GetValue<string>($"{ResendSettings.SectionName}:ApiToken") ?? string.Empty;
});
builder.Services.AddTransient<IResend, ResendClient>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<GoalService>();
builder.Services.AddScoped<HabitLocationService>();
builder.Services.AddScoped<SimpleHabitService>();
builder.Services.AddScoped<HabitService>();
builder.Services.AddScoped<CheckInService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<CalendarService>();
builder.Services.AddScoped<OnboardingService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<TaskItemService>();
builder.Services.AddScoped<IEmailService, ResendEmailService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
