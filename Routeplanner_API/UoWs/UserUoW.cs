using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Enums;
using Routeplanner_API.Helpers;
using Routeplanner_API.Models;

namespace Routeplanner_API.UoWs
{
    /// <summary>
    /// Unit of Work for user-related operations, handling user CRUD and authentication logic.
    /// </summary>
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

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>Enumerable of user DTOs.</returns>
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            _logger.LogInformation("Getting all users");

            var users = await _userDbQueries.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <returns>StatusCodeResponseDto with the user DTO or not found message.</returns>
        public async Task<StatusCodeResponseDto<UserDto?>> GetUsersByIdAsync(Guid userId)
        {
            _logger.LogInformation("Getting user with ID: {userId}", userId);

            User? user = await _userDbQueries.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID: {userId} not found", userId);
                return CreateStatusResponseDto<UserDto?>(StatusCodeResponse.NotFound, $"User with ID {userId} not found.", null);
            }

            return CreateStatusResponseDto<UserDto?>(StatusCodeResponse.Success, $"User with ID {userId} found.", _mapper.Map<UserDto>(user));
        }

        /// <summary>
        /// Creates a new user with the provided data.
        /// </summary>
        /// <param name="createUserDto">DTO containing user creation info.</param>
        /// <returns>StatusCodeResponseDto with JWT token string on success or error message.</returns>
        public async Task<StatusCodeResponseDto<string?>> CreateUserAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating new user");

            var validateUsernameAndEmail = await ValidateIfUsernameAndEmailAreUnique(createUserDto);
            if (validateUsernameAndEmail == null)
            {
                User? userEntity = _mapper.Map<User>(createUserDto);
                userEntity.Id = Guid.NewGuid();

                userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, createUserDto.Password);

                UserPermission? userPermission = await GetUserRight();
                if (userPermission == null)
                {
                    _logger.LogInformation("Error creating user: no userRightId in the database.");
                    return CreateStatusResponseDto<string?>(StatusCodeResponse.InternalServerError, "Error creating user: no userRightId in the database.", null);
                }

                userEntity.UserRightId = userPermission.Id;
                User? createdUser = await _userDbQueries.CreateAsync(userEntity);

                _logger.LogInformation($"User created successfully with ID: {createdUser.Id}");
                return CreateStatusResponseDto<string?>(StatusCodeResponse.Created, $"User created successfully with ID: {createdUser.Id}", GenerateUserJwtToken(_mapper.Map<UserDto>(createdUser)));
            }
            _logger.LogError(validateUsernameAndEmail);
            return CreateStatusResponseDto<string?>(StatusCodeResponse.BadRequest, validateUsernameAndEmail, null);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="receivedUserDto">User DTO with login credentials.</param>
        /// <returns>StatusCodeResponseDto with JWT token or error message.</returns>
        public async Task<StatusCodeResponseDto<string?>> LoginUserAsync(UserDto receivedUserDto)
        {
            User? foundUser = await FindUserByUsername(receivedUserDto.Username);

            if (foundUser == null)
            {
                return CreateStatusResponseDto<string?>(StatusCodeResponse.BadRequest, "Invalid username", null);
            }

            PasswordVerificationResult verificationResult = _passwordHasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash!, receivedUserDto.Password);

            if (verificationResult == PasswordVerificationResult.Success)
            {
                return CreateStatusResponseDto<string?>(StatusCodeResponse.Success, "Login successful", GenerateUserJwtToken(_mapper.Map<UserDto>(foundUser)));
            }

            return CreateStatusResponseDto<string?>(StatusCodeResponse.BadRequest, "Invalid password", null);
        }

        /// <summary>
        /// Updates an existing user with new data.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="updateUserDto">DTO containing update info.</param>
        /// <returns>StatusCodeResponseDto with updated user DTO or error message.</returns>
        public async Task<StatusCodeResponseDto<UserDto?>> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with ID: {userId}", userId);

            User? existingUser = await _userDbQueries.GetByIdAsync(userId);
            if (existingUser == null)
            {
                _logger.LogWarning($"User with ID: {userId} not found for update");
                return CreateStatusResponseDto<UserDto?>(StatusCodeResponse.NotFound, $"User with ID: {userId} not found for update", null);
            }

            string? validateUsernameAndEmail = await ValidateIfUsernameAndEmailAreUnique(updateUserDto);
            if (validateUsernameAndEmail == null)
            {
                _mapper.Map(updateUserDto, existingUser);

                User? updatedUser = await _userDbQueries.UpdateAsync(existingUser);
                _logger.LogInformation("User with ID: {userId} updated successfully", userId);

                return CreateStatusResponseDto<UserDto?>(StatusCodeResponse.Success, "User with ID: {userId} updated successfully", _mapper.Map<UserDto>(updatedUser));
            }

            _logger.LogError(validateUsernameAndEmail);
            return CreateStatusResponseDto<UserDto?>(StatusCodeResponse.BadRequest, validateUsernameAndEmail, null);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <returns>StatusCodeResponseDto indicating success or failure.</returns>
        public async Task<StatusCodeResponseDto<bool>> DeleteUserAsync(Guid userId)
        {
            _logger.LogInformation("Deleting user with ID: {userId}", userId);

            bool result = await _userDbQueries.DeleteAsync(userId);
            if (result)
            {
                _logger.LogInformation("User with ID: {userId} deleted successfully", userId);
                return CreateStatusResponseDto<bool>(StatusCodeResponse.Success, $"User with ID: {userId} deleted successfully", true);
            }
            else
            {
                _logger.LogWarning("User with ID: {userId} not found for deletion", userId);
                return CreateStatusResponseDto<bool>(StatusCodeResponse.NotFound, $"User with ID: {userId} not found for deletion", false);
            }
        }

        /// <summary>
        /// Creates a standardized status response DTO.
        /// </summary>
        public StatusCodeResponseDto<T> CreateStatusResponseDto<T>(StatusCodeResponse statusCodeResponse, string? message, T? data)
        {
            return new StatusCodeResponseDto<T>
            {
                StatusCodeResponse = statusCodeResponse,
                Message = message,
                Data = data
            };
        }

        private string GenerateUserJwtToken(UserDto user)
        {
            return _userHelper.GenerateUserJwtToken(user);
        }

        private async Task<User?> FindUserByUsername(string username)
        {
            return await _userDbQueries.FindUserByUsername(username);
        }

        private async Task<User?> FindUserByEmail(string email)
        {
            return await _userDbQueries.FindUserByEmail(email);
        }

        private async Task<string?> ValidateIfUsernameAndEmailAreUnique(CreateUserDto createUserDto)
        {
            var userFoundByUsername = await FindUserByUsername(createUserDto.Username);
            var userFoundByEmail = await FindUserByEmail(createUserDto.Email);

            if (userFoundByUsername != null && userFoundByEmail != null)
            {
                return "A user with this username and email already exists.";
            }
            else if (userFoundByUsername != null)
            {
                return "Username is already taken.";
            }
            else if (userFoundByEmail != null)
            {
                return "Email is already taken.";
            }
            return null;
        }

        private async Task<string?> ValidateIfUsernameAndEmailAreUnique(UpdateUserDto updateUserDto)
        {
            var userByUsername = await FindUserByUsername(updateUserDto.Username!);
            var userByEmail = await FindUserByEmail(updateUserDto.Email!);

            if (userByUsername != null && userByEmail != null)
            {
                return "A user with this username and email already exists.";
            }
            else if (userByUsername != null)
            {
                return "Username is already taken.";
            }
            else if (userByEmail != null)
            {
                return "Email is already taken.";
            }
            return null;
        }

        private async Task<UserPermission?> GetUserRight()
        {
            return await _userDbQueries.GetUserRight();
        }
    }
}
