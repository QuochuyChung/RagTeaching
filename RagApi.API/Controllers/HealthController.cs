using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("/health-checking")]
    public IActionResult CheckHealth()
    {
        return Ok("Chạy được rồi nha");
    }    
}
