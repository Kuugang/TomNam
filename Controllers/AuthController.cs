using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Middlewares;
using System.Text.Json;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly JwtAuthenticationService _jwtAuthenticationService;

    public AuthController(UserManager<User> userManager, IConfiguration configuration, JwtAuthenticationService jwtAuthenticationService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _jwtAuthenticationService = jwtAuthenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var userByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userByEmail != null)
        {
            return BadRequest($"User with email {request.Email} already exists.");
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Unable to register user {request.Email}: {GetErrorsText(result.Errors)}");
        }

        await _userManager.AddToRoleAsync(user, (request.UserRole).ToString());
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        User? user = null;

        user = await _userManager.FindByEmailAsync(request.Email);

        if (user != null)
        {
            var token = _jwtAuthenticationService.GenerateToken(user.Id, user.UserName, (await _userManager.GetRolesAsync(user))[0]);
            return Ok(new { token });
        }

        return BadRequest(new { message = "Invalid credentials." });
    }

    private string GetErrorsText(IEnumerable<IdentityError> errors)
    {
        return string.Join(", ", errors.Select(error => error.Description));
    }
}