using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.Database_Queries;
using System.Text.Json;

namespace Routeplanner_API.Controllers
{
    [ApiController]
    public class RouteController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Route[]> GetRoutes()
        {
            var routes = UoWs.RouteUoW.GetRoutes();

            if (routes == null || !routes.Any())
            {
                return NotFound("No routes found.");
            }

            return Ok(routes);
        }

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
