using Backend_Test.Application.Behaviors;
using Backend_Test.Application.CommandHandlers;
using Backend_Test.Application.Validators;
using Backend_Test.Domain.Interfaces;
using Backend_Test.Infrastructure.Persistence;
using DbUp;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using System.Reflection;
using SQLitePCL;
using static System.Net.Mime.MediaTypeNames;
using Backend_Test.Infrastructure.Persistence.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
DapperExtensions.DapperExtensions.SetMappingAssemblies(new[]
{
    typeof(DriverMap).Assembly // <-- Ensure this points to your Mappings namespace
});
// SQLite initialization (fixes your error)
SQLitePCL.Batteries.Init();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Ensure Data folder exists
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "Data"));
builder.Services.AddSingleton(new ApplicationDbContext(connectionString));
// Load Infrastructure assembly explicitly
var infrastructureAssembly = typeof(Backend_Test.Infrastructure.Persistence.UnitOfWork).Assembly;
var upgrader =
    DeployChanges.To
        .SqliteDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(infrastructureAssembly)
        .LogToConsole()
        .Build();

var result = upgrader.PerformUpgrade();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDriverCommandHandler).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateDriverCommandValidator).Assembly);

// Register ValidationBehavior for MediatR pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Driver Management API",
        Version = "v1",
        Description = "An API for managing drivers using Clean Architecture & CQRS"
    });

    // Optionally, include XML comments for improved Swagger docs:
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }