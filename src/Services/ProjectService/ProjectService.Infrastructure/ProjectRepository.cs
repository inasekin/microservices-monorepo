using Microsoft.EntityFrameworkCore;
using ProjectService.Domain.Models;

namespace ProjectService.Infrastructure
{
    public class ProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetAsync(Guid id)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var proj = await _context.Projects.FindAsync(id);
            if (proj != null)
            {
                _context.Projects.Remove(proj);
                await _context.SaveChangesAsync();
            }
        }
    }
}