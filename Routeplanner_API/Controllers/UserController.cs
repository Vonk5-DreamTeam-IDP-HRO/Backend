using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.UoWs;
using System.Text.Json;

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
        public ActionResult<User[]> GetUsers()
        {
            try
            {
                User[] users = _userUoW.GetUsers();
                if (users == null || users.Length == 0)
                {
                    _logger.LogWarning("GetUsers returned null, possibly due to a database issue.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve users.");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] JsonElement jsonBody)
        {
            try
            {
                _userUoW.AddUser(jsonBody);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("User added successfully.");
        }
    }
}
