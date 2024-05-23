using COS730.Helpers;
using COS730.Helpers.Interfaces;
using COS730.Models.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add configuration files to the builder
var config = builder.Configuration;

config.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<SQLConnectionSettings>(config.GetSection("SQLConnectionSettings"));
builder.Services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
builder.Services.Configure<EncryptionSettings>(config.GetSection("EncryptionSettings"));

builder.Services.AddSingleton<IEmailHelper, EmailHelper>();
builder.Services.AddSingleton<IEncryptionHelper, EncryptionHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
