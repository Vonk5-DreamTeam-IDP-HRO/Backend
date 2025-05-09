using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.Database_Queries;
using System.Text.Json;
using Routeplanner_API.UoWs;

namespace Routeplanner_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly RouteUoW _routeUoW;

        public RouteController(IConfiguration configuration)
        {
            _routeUoW = new RouteUoW(configuration);
        }

        [HttpGet]
        public ActionResult<Route[]> GetRoutes()
        {
            var routes = _routeUoW.GetRoutes();

            if (routes == null || !routes.Any())
            {
                return NotFound("No routes found.");
            }

            return Ok(routes);
        }

        [HttpPost]
        public IActionResult AddRoute([FromBody] JsonElement jsonBody)
        {
            try
            {
                _routeUoW.AddRoute(jsonBody);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Route added successfully.");
        }
    }
}
