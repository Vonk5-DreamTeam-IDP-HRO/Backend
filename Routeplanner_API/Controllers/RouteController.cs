using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Routeplanner_API.Controllers
{
    [ApiController]
    public class RouteController : ControllerBase
    {     
        [HttpPost]
        public IActionResult AddLocation([FromBody] JsonElement jsonBody)
        {
            try
            {
                UoWs.RouteUoW.AddRoute(jsonBody);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Route added successfully.");
        }
        
    }
}
