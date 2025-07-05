using System;
using Data.Context;
using Data.Mappers;
using Khabri;
using Microsoft.EntityFrameworkCore;
using Rido.Data;
using Service;
using Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(
    typeof(UserProfile),
    typeof(NewsProfile),
    typeof(ReportProfile)
);

builder.Services.AddHttpClient("KhabriClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "KhabriApp/1.0");
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSettings = jwtSection.Get<JwtSettings>();
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

builder.Services.AddAutoMapper(typeof(Data.Mappers.NewsProfile));
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureServices();
builder.Services.NewsSettingsProvider(builder.Configuration);
builder.Services.ConfigureRepositories();





builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    Logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}{context.Request.QueryString}");
    context.Request.EnableBuffering();
    string requestBody = "";
    if (context.Request.ContentLength > 0)
    {
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }
    }
    Logger.LogInformation($"Request Body: {requestBody}");
    var originalBodyStream = context.Response.Body;
    using var responseBody = new MemoryStream();
    context.Response.Body = responseBody;

    await next();
    context.Response.Body.Seek(0, SeekOrigin.Begin);
    string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
    context.Response.Body.Seek(0, SeekOrigin.Begin);

    Logger.LogInformation($"Response: {context.Response.StatusCode}");
    Logger.LogInformation($"Response Body: {responseText}");

    await responseBody.CopyToAsync(originalBodyStream);
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
