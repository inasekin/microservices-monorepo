using UserService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace UserService.Infrastructure
{
    public class UserProfileRepository
    {
        private readonly ApplicationDbContext _context;
        public UserProfileRepository(ApplicationDbContext context) => _context = context;

        public async Task<UserProfile?> GetProfileByIdAsync(Guid id)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProfileAsync(UserProfile profile)
        {
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }
    }
}