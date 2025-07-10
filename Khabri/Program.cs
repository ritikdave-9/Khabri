using Data.Context;
using Data.Mappers;
using Khabri;
using Khabri.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Utils;
using Rido.Data;
using Services;

// --- Create builder and configure logging ---
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

// --- Configure DbContext ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Configure AutoMapper ---
builder.Services.AddAutoMapper(
    typeof(UserProfile),
    typeof(NewsProfile),
    typeof(ReportProfile),
    typeof(Program)
);

// --- Configure HttpClient ---
builder.Services.AddHttpClient("KhabriClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "KhabriApp/1.0");
});

// --- Configure JWT Authentication ---
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

// --- Configure Dependency Injection for Services and Repositories ---
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureServices();
builder.Services.NewsSettingsProvider(builder.Configuration);
builder.Services.ConfigureRepositories();

// --- Configure CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// --- Add Controllers, Routing, Swagger ---
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Build the app ---
var app = builder.Build();

// --- Configure Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
