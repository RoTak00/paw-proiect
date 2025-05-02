using Microsoft.EntityFrameworkCore;

namespace PAW_Project.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}