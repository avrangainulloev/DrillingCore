using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : Controller
    {
        [HttpGet("server-date")]
        public IActionResult GetServerDate()
        {
            return Ok(new { now = DateTime.UtcNow });
        }
    }
}
