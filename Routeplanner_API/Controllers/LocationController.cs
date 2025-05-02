using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Routeplanner_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Location[]> GetLocations()
        {
            var locations = UoWs.LocationUoW.GetLocations();

            if (locations == null || !locations.Any())
            {
                return NotFound("No locations found.");
            }
            return Ok(locations);
        }
            
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

        [HttpPost]
        public IActionResult AddLocationDetails([FromBody] JsonElement jsonBody)
        {
            try
            {
                UoWs.LocationUoW.AddLocationDetails(jsonBody);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Location added successfully.");
        }
    }
}
