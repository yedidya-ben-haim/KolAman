using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace OperationalComponent.Services;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AlertEntity> Alerts { get; set; }
}
