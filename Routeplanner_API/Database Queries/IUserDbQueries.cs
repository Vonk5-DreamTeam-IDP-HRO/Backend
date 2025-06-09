using Routeplanner_API.Models;

namespace Routeplanner_API.Database_Queries
{
    public interface IUserDbQueries
    {
        Task<User> CreateAsync(User user);
        Task<User?> UpdateAsync(User user); 
        Task<bool> DeleteAsync(Guid userId);
        Task<User?> GetByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> FindUserByUsername(string username);
        Task<User?> FindUserByEmail(string email);
        Task<UserPermission?> GetUserRight();
    }
}
