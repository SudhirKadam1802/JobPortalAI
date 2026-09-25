using CareerAI.API.Middleware;
using CareerAI.Application.Common.Security;
using CareerAI.Application.Features.AIChat;
using CareerAI.Application.Features.AIInterview;
using CareerAI.Application.Features.Applications;
using CareerAI.Application.Features.Auth;
using CareerAI.Application.Features.CandidateProfile.Services;
using CareerAI.Application.Features.JobAlerts.Services;
using CareerAI.Application.Features.JobRecommendations;
using CareerAI.Application.Features.Jobs;
using CareerAI.Application.Features.Notifications;
using CareerAI.Application.Features.RecruiterDashboard.Services;
using CareerAI.Application.Features.Resumes;
using CareerAI.Application.Features.Resumes.Analysis;
using CareerAI.Application.Features.SavedJobs.Services;
using CareerAI.Application.Interfaces;
using CareerAI.Infrastructure.AI;
using CareerAI.Infrastructure.Authentication;
using CareerAI.Infrastructure.FileStorage;
using CareerAI.Infrastructure.Persistence;
using CareerAI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Database - Entity Framework Core
// ========================================

builder.Services.AddDbContext<CareerAIDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================================
// Repository Dependency Injection
// ========================================

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICandidateProfileRepository,CandidateProfileRepository>();
builder.Services.AddScoped<IRecruiterRepository, RecruiterRepository>();
builder.Services.AddScoped<ICandidateProfileService,CandidateProfileService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ISavedJobRepository, SavedJobRepository>();
builder.Services.AddScoped<ISavedJobService, SavedJobService>();
builder.Services.AddScoped<IJobAlertRepository, JobAlertRepository>();
builder.Services.AddScoped<IJobAlertService, JobAlertService>();
builder.Services.AddScoped<IAIChatRepository, AIChatRepository>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<INotificationRepository,NotificationRepository>();
builder.Services.AddScoped<INotificationService,NotificationService>();
builder.Services.AddScoped<IRecruiterDashboardRepository,RecruiterDashboardRepository>();
builder.Services.AddScoped<IRecruiterDashboardService,RecruiterDashboardService>();
builder.Services.AddScoped<IEmbeddingService, OllamaEmbeddingService>();
builder.Services.AddScoped<IRagRepository, RagRepository>();
builder.Services.AddScoped<IRagService, RagService>();
builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
builder.Services.AddScoped<IJobRecommendationRepository,JobRecommendationRepository>();
builder.Services.AddScoped<IResumeAnalysisRepository, ResumeAnalysisRepository>();
builder.Services.AddScoped<IResumeAnalysisService, ResumeAnalysisService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IJobRecommendationService,JobRecommendationService>();
builder.Services.AddScoped<IAIChatService, AIChatService>();
builder.Services.AddScoped<IAIInterviewRepository, AIInterviewRepository>();
builder.Services.AddScoped<IAIInterviewRepository, AIInterviewRepository>();
builder.Services.AddScoped<IAIInterviewService, AIInterviewService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IResumeTextExtractor, PdfResumeTextExtractor>();


// ========================================
// Authentication Services
// ========================================

builder.Services.AddScoped<PasswordHasher>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddHttpClient<IOllamaService, OllamaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(5);
});

// ========================================
// JWT Authentication
// ========================================

var jwtSettings = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSettings["Key"]
    ?? throw new InvalidOperationException("JWT Key is missing.");

var jwtIssuer = jwtSettings["Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing.");

var jwtAudience = jwtSettings["Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing.");

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ========================================
// Controllers
// ========================================

builder.Services.AddControllers();

// ========================================
// Swagger
// ========================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        document => new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// ========================================
// Build Application
// ========================================

var app = builder.Build();

app.UseCors("AllowAngular");

// ========================================
// HTTP Request Pipeline
// ========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();