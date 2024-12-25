using UserService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace UserService.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    }
}