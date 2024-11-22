using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using TomNam.Data;
using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Middlewares;
using TomNam.Middlewares.Filters;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly JwtAuthenticationService _jwtAuthenticationService;
    private readonly DataContext _context;

    public AuthController(UserManager<User> userManager, IConfiguration configuration, JwtAuthenticationService jwtAuthenticationService, DataContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _jwtAuthenticationService = jwtAuthenticationService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var userByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userByEmail != null)
        {
            return StatusCode(StatusCodes.Status409Conflict,
                new ResponseDTO { Message = $"User with email {request.Email} already exists." }
            );
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
                new ResponseDTO
                {
                    Message = "Something went wrong with user creation. Please try again",
                }
            );
        }

        await _userManager.AddToRoleAsync(user, (request.UserRole).ToString());

        switch (request.UserRole)
        {
            case "Customer":
                CustomerProfile CustomerProfile = new CustomerProfile
                {
                    UserId = user.Id,
                    User = user,
                    BehaviorScore = 30
                };
		        await _context.CustomerProfile.AddAsync(CustomerProfile);
                break;
            case "Owner":
                OwnerProfile OwnerProfile = new OwnerProfile
                {
                    UserId = user.Id,
                    User = user
                };
		        await _context.OwnerProfile.AddAsync(OwnerProfile);
                break;
        }

		await _context.SaveChangesAsync();
        
        return StatusCode(StatusCodes.Status201Created,
            new ResponseDTO
            {
                Message = "User created successfully",
                Data = new JWTDTO {
                    Token = _jwtAuthenticationService.GenerateToken(user.Id, user.UserName, (await _userManager.GetRolesAsync(user))[0])
                }
            }
        );
    }

    [HttpPost("login")]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // if (!ModelState.IsValid)
        // {
        //     // return BadRequest(new { message = "Invalid request." });
        //     // custom response
        // }

        User? user = null;

        user = await _userManager.FindByEmailAsync(request.Email);

        if (user != null)
        {
            var token = _jwtAuthenticationService.GenerateToken(user.Id, user.UserName, (await _userManager.GetRolesAsync(user))[0]);
            Console.WriteLine(token.GetType());
            Console.WriteLine(token);

            return Ok(
                new ResponseDTO
                {
                    Message = "Login successful",
                    Data = new JWTDTO { Token = token }
                }
            );
        }

        return BadRequest(new { message = "Invalid credentials." });
    }

    private string GetErrorsText(IEnumerable<IdentityError> errors)
    {
        return string.Join(", ", errors.Select(error => error.Description));
    }
}