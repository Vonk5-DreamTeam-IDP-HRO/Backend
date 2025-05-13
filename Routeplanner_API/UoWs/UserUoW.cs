using AutoMapper;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.Helpers;
using Routeplanner_API.Models;

namespace Routeplanner_API.UoWs
{
    public class UserUoW
    {
        private readonly IUserDbQueries _userDbQueries;
        private readonly IUserHelper _userHelper;
        private readonly IMapper _mapper;
        private readonly ILogger<UserUoW> _logger;

        public UserUoW(IUserDbQueries userDbQueries, IUserHelper userHelper, IMapper mapper, ILogger<UserUoW> logger)
        {
            _userDbQueries = userDbQueries ?? throw new ArgumentNullException(nameof(userDbQueries));
            _userHelper = userHelper ?? throw new ArgumentNullException(nameof(userHelper));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            _logger.LogInformation("Getting all users");
            var users = await _userDbQueries.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUsersByIdAsync(int userId)
        {
            _logger.LogInformation("Getting user with ID: {userId}", userId);

            var user = await _userDbQueries.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID: {userId} not found", userId);
                return null;
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> FindUserByEmailAsync(string email)
        {
            _logger.LogInformation("Finding user with email: {email}", email);

            UserConfidential? user = await _userDbQueries.FindUserByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("User with email: {email} not found", email);
                return null;
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> CheckPasswordAsync(UserDto user, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> CreateUserAsync(UserDto createUserDto)
        {
            _logger.LogInformation("Creating new user");

            try
            {
                var userEntity = _mapper.Map<User>(createUserDto);

                // Potentially set UserId if applicable and not directly from DTO
                // userEntity.UserId = ...; 

                var createdUser = await _userDbQueries.CreateAsync(userEntity);
                _logger.LogInformation("User created successfully with ID: {userId}", createdUser.UserId);
                return _mapper.Map<UserDto>(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with ID: {userId}", userId);

            var existingUser = await _userDbQueries.GetByIdAsync(userId);
            if (existingUser == null)
            {
                _logger.LogWarning("User with ID: {userId} not found for update", userId);
                return null;
            }

            try
            {
                // Map the changes from DTO to the existing entity
                _mapper.Map(updateUserDto, existingUser);

                // Ensure UpdatedAt is set (AutoMapper profile also does this, but explicit here is fine too)
                // existingUser.UpdatedAt = DateTime.UtcNow;

                var updatedUser = await _userDbQueries.UpdateAsync(existingUser);
                _logger.LogInformation("User with ID: {userId} updated successfully", userId);
                return _mapper.Map<UserDto>(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {userId}: {ErrorMessage}", userId, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            _logger.LogInformation("Deleting user with ID: {userId}", userId);
            try
            {
                var result = await _userDbQueries.DeleteAsync(userId);
                if (result)
                {
                    _logger.LogInformation("User with ID: {userId} deleted successfully", userId);
                }
                else
                {
                    _logger.LogWarning("User with ID: {userId} not found for deletion", userId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {userId}: {ErrorMessage}", userId, ex.Message);
                throw;
            }
        }

        public string GenerateUserJwtToken(UserDto user)
        {
            return _userHelper.GenerateUserJwtToken(user);
        }

        public string GenerateAdminJwtToken(UserDto user)
        {
            return _userHelper.GenerateAdminJwtToken(user);
        }
    }
}
