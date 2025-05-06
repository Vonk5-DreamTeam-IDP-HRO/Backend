using Routeplanner_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Routeplanner_API.Data.Repositories
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(int locationId);
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location> CreateAsync(Location location);
        Task<Location?> UpdateAsync(Location location); // Returns null if not found
        Task<bool> DeleteAsync(int locationId);
    }
}
