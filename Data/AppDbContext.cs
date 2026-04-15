using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpsBoard.API.Models;

namespace OpsBoard.API.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Resource> Resources { get; set; }
    public DbSet<Organization> Organizations { get; set; }
}