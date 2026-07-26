using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SibersTest.Application.Interfaces;
using SibersTest.Application.Services;
using SibersTest.Application.Validators;
using SibersTest.Data;
using SibersTest.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DbConnect")));

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeRequestValidator>();

// Register repositories
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();

// Register services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectTaskService, ProjectTaskService>();
>>>>>>>


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api-docs");
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();