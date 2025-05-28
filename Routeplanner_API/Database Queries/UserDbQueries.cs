using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;

namespace Routeplanner_API.Database_Queries
{
    public class UserDbQueries : IUserDbQueries
    {
        private readonly RouteplannerDbContext _context;

        public UserDbQueries(RouteplannerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                return null;
            }
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteAsync(Guid userId)
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

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> FindUserByUsername(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User?> FindUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserPermission?> GetUserRight()
        {
            return await _context.UserRights.FirstOrDefaultAsync(u => u.Name == "user");
        }
    }
}
