using Microsoft.AspNetCore.Mvc;

using TomNam.Models.DTO;
using TomNam.Interfaces;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _userService.Register(request);
        return StatusCode(StatusCodes.Status201Created, response);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _userService.Login(request);
        return StatusCode(StatusCodes.Status200OK, response);
    }
}