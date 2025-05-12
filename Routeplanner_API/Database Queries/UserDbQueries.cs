using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;
using Routeplanner_API.Data;

namespace Routeplanner_API.Database_Queries
{
    public class UserDbQueries : IUserDbQueries
    {
        private readonly RouteplannerDbContext _context;

        public UserDbQueries(RouteplannerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserConfidential?> FindUserByEmailAsync(string email)
        {
            return await _context.UserConfidentials.FirstOrDefaultAsync(u=> u.Email == email); // wut? user heeft geen email veld.
        }

        public async Task<User> CreateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var existingUser = await _context.Users.FindAsync(user.UserId);
            if (existingUser == null)
            {
                return null;
            }

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var userToDelete = await _context.Users.FindAsync(userId);
            if (userToDelete == null)
            {
                return false;
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
