using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.DTO.User;
using Routeplanner_API.UoWs;

namespace Routeplanner_API.Controllers
{
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

        [HttpGet]
        [Authorize]
        // Should be only accessible by admins or special authorized people
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            _logger.LogInformation("Executing UserController.GetUsers");
            try
            {
                var users = await _userUoW.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving users.");
            }
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
        {
            _logger.LogInformation("Executing UserController.GetUserById");
            try
            {
                var user = await _userUoW.GetUsersByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogInformation("User with ID {userId} not found.", userId);
                    return NotFound($"User with ID {userId} not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with ID {userId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred while retrieving user {userId}.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateUserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            _logger.LogInformation("Executing UserController.CreateUser");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateUser called with invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdUser = await _userUoW.CreateUserAsync(createUserDto);

                var token = _userUoW.GenerateUserJwtToken(createdUser);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new user. Input: {@createUserDto}", createUserDto);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the user.");
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginUser([FromBody] UserDto userDto)
        {
            _logger.LogInformation("Executing UserController.LoginUser");

            var result = await _userUoW.LoginUserAsync(userDto);

            if (!result.Success)
            {
                return Unauthorized(result.Message);
            }
            var token = _userUoW.GenerateUserJwtToken(userDto);
            return Ok(new { Token = token });
        }

        [HttpPut("{userId}")]
        [Authorize]
        // Should be only accessible by admins or special authorized people
        // as example using: [Authorize (Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LocationDto>> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Executing UserController.UpdateUser");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateUser called with invalid model state for ID {userId}.", userId);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedUser = await _userUoW.UpdateUserAsync(userId, updateUserDto);
                if (updatedUser == null)
                {
                    _logger.LogWarning("Attempted to update non-existent user with ID {userId}.", userId);
                    return NotFound($"User with ID {userId} not found for update.");
                }
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {userId}. Input: {@updateUserDto}", userId, updateUserDto);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred while updating user {userId}.");
            }
        }

        [HttpDelete("{userId}")]
        [Authorize]
        // Should be only accessible by admins or special authorized people
        // as example using: [Authorize (Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            _logger.LogInformation("Executing UserController.DeleteUser");
            try
            {
                var success = await _userUoW.DeleteUserAsync(userId);
                if (!success)
                {
                    _logger.LogWarning("Attempted to delete non-existent user with ID {userId}.", userId);
                    return NotFound($"User with ID {userId} not found for deletion.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {userId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred while deleting user {userId}.");
            }
        }
    }
}
