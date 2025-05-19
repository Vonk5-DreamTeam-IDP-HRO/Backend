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
            ArgumentNullException.ThrowIfNull(user);

            var existingUser = await _context.Users.FindAsync(user.UserId);
            if (existingUser == null)
            {
                return null;
            }

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
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserConfidential?> FindUserByUsername(string username)
        {
            var temp = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (temp == null)
                return null;
            return null;
            //return new UserConfidential // not working yet properly. no username?
            //{
            //    UserId = temp.UserId,
            //    Email = temp.Email,
            //    PasswordHash = temp.PasswordHash,
            //    User = temp.Username
            //};

            // TODO: fix returning the UserConfidential object
        }
    }
}
