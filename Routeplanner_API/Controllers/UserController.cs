using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Routeplanner_API.DTO;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Routeplanner_API.Controllers
{
    [ApiController]
    [Authorize]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> GetUserById(int userId)
        {
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
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateUser called with invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdUser = await _userUoW.CreateUserAsync(createUserDto);
                //return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.UserId }, createdUser); // Old return statement

                var token = _userUoW.GenerateJwtToken(createdUser);
                
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new user. Input: {@createUserDto}", createUserDto);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the user.");
            }
        }

        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LocationDto>> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        {
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var user = await _userUoW.FindUserByEmailAsync(userDto.Email);
            if (user != null && await _userUoW.CheckPasswordAsync(user, userDto.PasswordHash))
                return Ok(new { Token = _userUoW.GenerateJwtToken(user) });
            return Unauthorized();
        }
    }
}
