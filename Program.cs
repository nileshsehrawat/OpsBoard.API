using Microsoft.EntityFrameworkCore;
using OpsBoard.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// DB Context (SQLite for now)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=opsboard.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
