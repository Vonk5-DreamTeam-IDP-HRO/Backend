using AutoMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.Helpers;
using Routeplanner_API.Models;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.User;

namespace Routeplanner_API.UoWs
{
    public class UserUoW
    {
        private readonly IUserDbQueries _userDbQueries;
        private readonly IUserHelper _userHelper;
        private readonly IMapper _mapper;
        private readonly ILogger<UserUoW> _logger;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

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

        public async Task<UserDto?> GetUsersByIdAsync(Guid userId)
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

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating new user");

            try
            {
                var userEntity = _mapper.Map<User>(createUserDto);

                // Hash the plain text password from the DTO and store it on the User entity
                userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, createUserDto.Password);

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

        public async Task<LoginDto> LoginUserAsync(UserDto receivedUserDto)
        {
            User? foundUser = await FindUserByUsername(receivedUserDto.Username);

            if (foundUser == null)
            {
                return new LoginDto
                {
                    Success = false,
                    Message = "Invalid username"
                };
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash, receivedUserDto.PasswordHash);

            if (verificationResult == PasswordVerificationResult.Success)
            {
                return new LoginDto
                {
                    Success = true,
                    Message = "Login successful"
                };
            }

            return new LoginDto
            {
                Success = false,
                Message = "Invalid password"
            };
        }

        public async Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
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

        public async Task<bool> DeleteUserAsync(Guid userId)
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

        public async Task<User?> FindUserByUsername(string username)
        {
            return await _userDbQueries.FindUserByUsername(username);
        }
      
        public string GenerateUserJwtToken(UserDto user)
        {
            return _userHelper.GenerateUserJwtToken(user);
        }
    }
}
