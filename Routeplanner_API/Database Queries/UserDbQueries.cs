using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;

namespace Routeplanner_API.Database_Queries
{
    /// <summary>
    /// Provides database operations related to User entities.
    /// </summary>
    public class UserDbQueries : IUserDbQueries
    {
        private readonly RouteplannerDbContext _context;

        public UserDbQueries(RouteplannerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new user to the database asynchronously.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>The created <see cref="User"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="user"/> is null.</exception>
        public async Task<User> CreateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Updates an existing user in the database asynchronously.
        /// </summary>
        /// <param name="user">The user entity with updated data.</param>
        /// <returns>The updated <see cref="User"/> if found; otherwise, null.</returns>
        public async Task<User?> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);

            _context.Entry(existingUser!).CurrentValues.SetValues(user);

            await _context.SaveChangesAsync();
            return existingUser;
        }

        /// <summary>
        /// Deletes a user by their unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to delete.</param>
        /// <returns><c>true</c> if the user was deleted; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Retrieves a user by their unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The <see cref="User"/> if found; otherwise, null.</returns>
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Retrieves all users from the database asynchronously.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="User"/> entities.</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Finds a user by their username asynchronously.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The <see cref="User"/> if found; otherwise, null.</returns>
        public async Task<User?> FindUserByUsername(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        /// <summary>
        /// Finds a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The <see cref="User"/> if found; otherwise, null.</returns>
        public async Task<User?> FindUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Retrieves the user permission named "user" asynchronously.
        /// </summary>
        /// <returns>The <see cref="UserPermission"/> if found; otherwise, null.</returns>
        public async Task<UserPermission?> GetUserRight()
        {
            return await _context.UserRights.FirstOrDefaultAsync(u => u.Name == "user");
        }
    }
}
