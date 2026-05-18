using Microsoft.EntityFrameworkCore;
using ShelfMaster.Application.Interfaces;
using ShelfMaster.Application.Services;
using ShelfMaster.Infrastructure.Data;
using ShelfMaster.Infrastructure.Repositories;
using ShelfMaster.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DB Context (Update connection string as needed)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Dependency Injection: Register the Repository and Service
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<InventoryService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // For testing via UI

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// 3. Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseExceptionHandler();

app.Run();