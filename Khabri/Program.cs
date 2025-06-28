using System;
using Data.Context;
using Data.Mappers;
using Khabri;
using Microsoft.EntityFrameworkCore;
using Rido.Data;
using Service;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddHttpClient("KhabriClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "KhabriApp/1.0");
});


builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureServices();
builder.Services.NewsSettingsProvider(builder.Configuration);
builder.Services.ConfigureRepositories();
builder.Services.AddHostedService<NewsSourceBackgroundService>();



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
app.UseAuthorization();

app.MapControllers();

app.Run();
