using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Routeplanner_API.Controllers
{
    [ApiController]
    public class LocationController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddLocation([FromBody] JsonElement jsonBody)
        {
            try
            {
                UoWs.LocationUoW.AddLocation(jsonBody);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Location added successfully.");
        }
    }
}
