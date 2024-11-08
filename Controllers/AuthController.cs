using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TomNam.Models;
using TomNam.Models.DTO;
using System.Text.Json;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
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

        User user = null;

        if (request.Email != null)
        {
            user = await _userManager.FindByEmailAsync(request.Email);
        }
        else
        {
            user = _userManager.Users.SingleOrDefault(u => u.PhoneNumber == request.Phone);
        }

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return BadRequest(new { message = "invalid credentials." });
        }

        var userDto = new LoginResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id,
            Role = (await _userManager.GetRolesAsync(user))[0],
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
        };

        var authClaims = new List<Claim>
        {
            new Claim("User", JsonSerializer.Serialize(new 
            {
                userDto.FirstName,
                userDto.LastName,
                userDto.Id,
                userDto.UserName,
                userDto.Email,
                userDto.EmailConfirmed,
                userDto.PhoneNumber,
                userDto.PhoneNumberConfirmed,
                userDto.TwoFactorEnabled
            })),
            new Claim(ClaimTypes.Role, userDto.Role),
        };

        var token = GenerateJwtToken(authClaims);

        var tokenResponse = new AccessTokenResponse
        {
            TokenType = "Bearer",
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn = (long)(token.ValidTo - DateTime.UtcNow).TotalSeconds,
            RefreshToken = null // TODO: Implement refresh token
        };

        return Ok(tokenResponse);
    }

    private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        return new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }

    private string GetErrorsText(IEnumerable<IdentityError> errors)
    {
        return string.Join(", ", errors.Select(error => error.Description));
    }
}
