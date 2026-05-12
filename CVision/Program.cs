using CVision.BLL.Commands.Users.Register;
using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Realizations.Base;
using CVision.BLL.Interfaces;
using CVision.BLL.Mappers;
using CVision.BLL.Options;
using CVision.BLL.Services;
using CVision.BLL.Validators.Users;
using CVision.Hubs;
using CVision.Mappers;
using CVision.Extensions;
using CVision.Extensions.Middleware;
using CVision.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

builder.Services.AddRepositoriesFromAssembly(typeof(RepositoryWrapper).Assembly);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddScoped<ITextExtractor, PdfTextExtractor>();
builder.Services.AddScoped<ITextExtractor, DocxTextExtractor>();
builder.Services.AddScoped<ITextExtractor, ImageTextExtractor>();
builder.Services.AddScoped<IGlassdoorProvider, GlassdoorProvider>();

builder.Services.AddScoped<ICvParserService, CvParserService>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IVacancyProvider, VacancyProvider>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddAutoMapper(typeof(UsersProfile), typeof(CVAnalysisProfile));
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserValidator).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

if (builder.Environment.IsProduction())
{
    var keyVaultName = builder.Configuration["KeyVaultName"];
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}

var cloudinaryOptions = builder.Configuration
    .GetSection("CloudinarySettings")
    .Get<CloudinaryOptions>();

builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("Caching"));

builder.Services.AddScoped<IFileService>(sp => new CloudinaryFileService(cloudinaryOptions!));

string geminiApiKey = builder.Configuration["GeminiApiKey"]
                      ?? throw new Exception("Gemini API Key is missing!");

builder.Services.AddScoped<IAIService>(sp => new GeminiService(geminiApiKey));

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddHostedService<NotificationBackgroundService>();
builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }


app.MapGet("/test-exception", () =>
{
    throw new Exception("Test exception");
});

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseRouting();
app.UseSession();

app.UseMiddleware<ResponseTimeMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();