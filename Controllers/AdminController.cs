using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]  // Restricts access to only users with the 'Admin' role
public class AdminController : ControllerBase
{
    // Your actions here
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok(new { message = "Admin dashboard data" });
    }
}