using ProjectService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectService.Infrastructure
{
    public class ProjectRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectRepository(ApplicationDbContext context) => _context = context;

        public async Task<Project?> GetProjectByIdAsync(Guid id)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }
    }
}