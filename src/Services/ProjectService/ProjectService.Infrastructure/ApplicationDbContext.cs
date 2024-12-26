using Microsoft.EntityFrameworkCore;
using ProjectService.Domain.Models;

namespace ProjectService.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; } = null!;
    }
}