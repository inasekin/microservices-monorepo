using ProjectService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectService.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<Project> Projects { get; set; } = null!;
    }
}