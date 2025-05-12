using Routeplanner_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Routeplanner_API.Database_Queries
{
    public interface IUserDbQueries
    {
        Task<User?> GetByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<UserConfidential?> FindUserByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User?> UpdateAsync(User user); // Returns null if not found
        Task<bool> DeleteAsync(int userId);
    }
}
