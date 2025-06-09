using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Enums;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;

namespace Routeplanner_API.Controllers
{
    /// <summary>
    /// API controller for managing user-related operations such as retrieval, creation, updating, and deletion of users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserUoW _userUoW;
        private readonly ILogger<UserController> _logger;

        public UserController(UserUoW userUoW, ILogger<UserController> logger)
        {
            _userUoW = userUoW ?? throw new ArgumentNullException(nameof(userUoW));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a list of all users from the system.
        /// </summary>
        /// <returns>A status response containing a list of <see cref="UserDto"/> objects.</returns>
        [HttpGet]
        [Authorize]
        // Should be only accessible by admins or special authorized people
        // as example using: [Authorize (Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<IEnumerable<UserDto>>>> GetUsers()
        {
            _logger.LogInformation("Executing UserController.GetUsers");
            try
            {
                var users = await _userUoW.GetUsersAsync();
                return _userUoW.CreateStatusResponseDto<IEnumerable<UserDto>>(StatusCodeResponse.Success, null, users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users.");
                return _userUoW.CreateStatusResponseDto<IEnumerable<UserDto>>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while retrieving users.", null);
            }
        }

        /// <summary>
        /// Retrieves a single user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>A status response containing the <see cref="UserDto"/> or an appropriate error message.</returns>
        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<UserDto?>>> GetUserById(Guid userId)
        {
            _logger.LogInformation("Executing UserController.GetUserById");
            try
            {
                return await _userUoW.GetUsersByIdAsync(userId);               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with ID {userId}.", userId);
                return _userUoW.CreateStatusResponseDto<UserDto?>(StatusCodeResponse.InternalServerError, $"An unexpected error occurred while retrieving user {userId}.", null);
            }
        }

        /// <summary>
        /// Creates a new user in the system using the provided data.
        /// </summary>
        /// <param name="createUserDto">The data required to create a new user.</param>
        /// <returns>A status response containing the result message or an appropriate error message.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<string?>>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            _logger.LogInformation("Executing UserController.CreateUser");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateUser called with invalid model state.");
                return _userUoW.CreateStatusResponseDto<string?>(StatusCodeResponse.BadRequest, "CreateUser called with invalid model state.", null);
            }

            try
            {
                return await _userUoW.CreateUserAsync(createUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new user. Input: {@createUserDto}", createUserDto);
                return _userUoW.CreateStatusResponseDto<string?>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while creating the user.", null);
            }
        }

        /// <summary>
        /// Authenticates a user based on the provided credentials.
        /// </summary>
        /// <param name="userDto">The user credentials for login.</param>
        /// <returns>A status response containing an authentication token or an appropriate error message.</returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<string?>>> LoginUser([FromBody] UserDto userDto)
        {            
            _logger.LogInformation("Executing UserController.LoginUser");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("LoginUser called with invalid model state.");
                return _userUoW.CreateStatusResponseDto<string?>(StatusCodeResponse.BadRequest, "LoginUser called with invalid model state.", null);
            }
            try
            {
                return await _userUoW.LoginUserAsync(userDto);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in a user. Input: {@userDto}", userDto);
                return _userUoW.CreateStatusResponseDto<string?>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while logging in the user.", null);
            }
        }

        /// <summary>
        /// Updates an existing user's information based on the provided data.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to update.</param>
        /// <param name="updateUserDto">The updated user information.</param>
        /// <returns>A status response containing the updated <see cref="UserDto"/> or an appropriate error message.</returns>
        [HttpPut("{userId}")]
        [Authorize]
        // Should be only accessible by admins or special authorized people
        // as example using: [Authorize (Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<UserDto?>>> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Executing UserController.UpdateUser");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateUser called with invalid model state for ID {userId}.", userId);
                return _userUoW.CreateStatusResponseDto<UserDto?>(StatusCodeResponse.BadRequest, "UpdateUser called with invalid model state.", null);
            }
            try
            {
                return await _userUoW.UpdateUserAsync(userId, updateUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {userId}. Input: {@updateUserDto}", userId, updateUserDto);
                return _userUoW.CreateStatusResponseDto<UserDto?>(StatusCodeResponse.InternalServerError, $"An unexpected error occurred while updating user {userId}.", null);
            }
        }

        /// <summary>
        /// Deletes a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to delete.</param>
        /// <returns>A status response indicating whether the deletion was successful or an appropriate error message.</returns>
        [HttpDelete("{userId}")]
        [Authorize]
        // Should be only accessible by admins or special authorized people
        // as example using: [Authorize (Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<bool>>> DeleteUser(Guid userId)
        {
            _logger.LogInformation("Executing UserController.DeleteUser");
            try
            {
                return await _userUoW.DeleteUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while deleting user {userId}.");
                return _userUoW.CreateStatusResponseDto<bool>(StatusCodeResponse.InternalServerError, $"An unexpected error occurred while deleting user {userId}.", false);
            }
        }
    }
}
