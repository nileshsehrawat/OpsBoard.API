using Microsoft.EntityFrameworkCore;
using OpsBoard.API.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB Context (SQLite for now)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=opsboard.db"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000", "http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
// app.UseHttpsRedirection();

// Use CORS BEFORE auth
app.UseCors("AllowFrontend");

// Authentication BEFORE Authorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
