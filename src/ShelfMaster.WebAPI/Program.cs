using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
builder.Services.AddScoped<StockTransactionService>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // For testing via UI

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["JwtSettings:Secret"] ?? "SuperSecretDefaultKeyThatIsAtLeast32BytesLong!"
        )),
        ValidateIssuer = false,   // Set to true if you configured an Issuer URL
        ValidateAudience = false, // Set to true if you configured an Audience URL
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 3. Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapControllers();

app.MapFallbackToFile("index.html");

app.UseExceptionHandler();

// =========================================================================
// AUTOMATIC DATABASE MIGRATIONS ON STARTUP
// =========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get your Entity Framework DbContext
        var context = services.GetRequiredService<AppDbContext>(); 
        
        // This automatically runs pending migrations and creates tables if they don't exist
        await context.Database.MigrateAsync();
        Console.WriteLine("🟢 Production Database successfully migrated and synced!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "🔴 An error occurred while migrating the database on startup.");
    }
}

app.Run();